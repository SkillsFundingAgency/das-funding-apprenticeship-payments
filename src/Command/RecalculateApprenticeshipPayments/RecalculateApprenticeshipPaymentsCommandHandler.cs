using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public class RecalculateApprenticeshipPaymentsCommandHandler : IRecalculateApprenticeshipPaymentsCommandHandler
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;

    public RecalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository,
        IDasServiceBusEndpoint busEndpoint,
        IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
        ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _busEndpoint = busEndpoint;
        _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
        _logger = logger;
    }

    public async Task Recalculate(RecalculateApprenticeshipPaymentsCommand command)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);

        apprenticeship.ClearEarnings();
        
        foreach (var earning in command.NewEarnings)
        {
            apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth, earning.FundingLineType, earning.EarningsProfileId);
        }

        apprenticeship.RecalculatePayments(DateTime.Now);

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _busEndpoint.Publish(@event);
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}