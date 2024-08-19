using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

public class ProcessUnfundedPaymentsCommandHandler : IProcessUnfundedPaymentsCommandHandler
{
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;
    private readonly ISystemClockService _systemClock;
    private readonly ILogger<ProcessUnfundedPaymentsCommandHandler> _logger;

    public ProcessUnfundedPaymentsCommandHandler(
        IDasServiceBusEndpoint busEndpoint, 
        IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder,
        ISystemClockService systemClock,
        ILogger<ProcessUnfundedPaymentsCommandHandler> logger)
    {
        _busEndpoint = busEndpoint;
        _eventBuilder = eventBuilder;
        _systemClock = systemClock;
        _logger = logger;
    }

    public async Task Process(ProcessUnfundedPaymentsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Model);
        var apprenticeshipKey = command.Model.ApprenticeshipKey;
        
        var paymentsToSend = GetPaymentsForRequestedCollectionPeriod(command);

        if (command.Model.PaymentsFrozen)
        {
            MarkPaymentsNotPaidDueToFreeze(paymentsToSend);
            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} - Payments are frozen, no payments will be published", apprenticeshipKey);
            return;
        }

        var previouslyFrozenPaymentsToSend = GetPreviouslyFrozenPayments(command);

        paymentsToSend = paymentsToSend.Union(previouslyFrozenPaymentsToSend).ToArray();

        if (paymentsToSend.Any())
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  Publishing {numberOfPayments} payments for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, paymentsToSend.Length, command.CollectionPeriod, command.CollectionYear);
        }
        else
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  No payments to publish for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, command.CollectionPeriod, command.CollectionYear);
        }

        foreach (var payment in paymentsToSend)
        {
            var finalisedOnProgammeLearningPaymentEvent = _eventBuilder.Build(payment, command.Model);
            await _busEndpoint.Publish(finalisedOnProgammeLearningPaymentEvent);

            _logger.LogInformation("ApprenticeshipKey: {0} Publishing FinalisedOnProgammeLearningPaymentEvent: {1}",
                finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey,
                finalisedOnProgammeLearningPaymentEvent.SerialiseForLogging());

            payment.SentForPayment = true;
            payment.NotPaidDueToFreeze = false;
        }
    }

    private PaymentEntityModel[] GetPaymentsForRequestedCollectionPeriod(ProcessUnfundedPaymentsCommand command)
    {
        return command.Model.Payments
                .Where(x => x.CollectionPeriod == command.CollectionPeriod && x.CollectionYear == command.CollectionYear && !x.SentForPayment)
                .ToArray();
    }

    private void MarkPaymentsNotPaidDueToFreeze(PaymentEntityModel[] payments)
    {
        foreach (var paymentEntityModel in payments)
        {
            paymentEntityModel.NotPaidDueToFreeze = true;
        }
    }

    private PaymentEntityModel[] GetPreviouslyFrozenPayments(ProcessUnfundedPaymentsCommand command)
    {
        var validAcademicYears = new List<short> { _systemClock.Now.ToAcademicYear() };

        if(command.HardCloseDate >= _systemClock.Now)
        {
            validAcademicYears.Add(command.PreviousAcademicYear);
        }

        var previouslyFrozenPaymentsToSend = command.Model.Payments
            .Where(x => x.NotPaidDueToFreeze && validAcademicYears.Contains(x.CollectionYear))
            .ToArray();

        foreach (var payment in previouslyFrozenPaymentsToSend)
        {
            payment.CollectionYear = command.CollectionYear;
            payment.CollectionPeriod = command.CollectionPeriod;
        }

        return previouslyFrozenPaymentsToSend;
    }
}