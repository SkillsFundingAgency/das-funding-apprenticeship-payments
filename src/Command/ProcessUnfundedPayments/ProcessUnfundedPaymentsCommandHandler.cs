using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

public class ProcessUnfundedPaymentsCommandHandler : IProcessUnfundedPaymentsCommandHandler
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;
    private readonly ISystemClockService _systemClock;
    private readonly ILogger<ProcessUnfundedPaymentsCommandHandler> _logger;

    public ProcessUnfundedPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository, IDasServiceBusEndpoint busEndpoint, IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder, ISystemClockService systemClock, ILogger<ProcessUnfundedPaymentsCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _busEndpoint = busEndpoint;
        _eventBuilder = eventBuilder;
        _systemClock = systemClock;
        _logger = logger;
    }

    public async Task Process(ProcessUnfundedPaymentsCommand command)
    {
        var apprenticeshipKey = command.ApprenticeshipKey;
        var apprenticeship = await _apprenticeshipRepository.Get(apprenticeshipKey);
        
        if (apprenticeship.PaymentsFrozen)
        {
            apprenticeship.MarkPaymentsAsFrozen(command.CollectionYear, command.CollectionPeriod);
            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} - Payments are frozen, no payments will be published", apprenticeshipKey);
            await _apprenticeshipRepository.Update(apprenticeship);
            return;
        }

        apprenticeship.UnfreezeFrozenPayments(command.CollectionYear, command.CollectionPeriod, _systemClock.Now.ToAcademicYear(), command.PreviousAcademicYear, command.HardCloseDate, _systemClock.Now);
        var paymentsToSend = apprenticeship.DuePayments(command.CollectionYear, command.CollectionPeriod);
        if (paymentsToSend.Any())
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  Publishing {numberOfPayments} payments for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, paymentsToSend.Count, command.CollectionPeriod, command.CollectionYear);
        }
        else
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  No payments to publish for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, command.CollectionPeriod, command.CollectionYear);
        }

        apprenticeship.MarkPaymentsAsSent(command.CollectionYear, command.CollectionPeriod);
        await _apprenticeshipRepository.Update(apprenticeship);

        foreach (var payment in paymentsToSend)
        {
            var finalisedOnProgammeLearningPaymentEvent = _eventBuilder.Build(payment, apprenticeship);
            await _busEndpoint.Publish(finalisedOnProgammeLearningPaymentEvent);

            _logger.LogInformation("ApprenticeshipKey: {0} Publishing FinalisedOnProgammeLearningPaymentEvent: {1}",
                finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey,
                finalisedOnProgammeLearningPaymentEvent.SerialiseForLogging());
        }
    }
}