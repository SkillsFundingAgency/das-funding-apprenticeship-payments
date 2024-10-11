using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public class RecalculateApprenticeshipPaymentsCommandHandler : IRecalculateApprenticeshipPaymentsCommandHandler
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;
    private readonly ISystemClockService _systemClockService;

    public RecalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository,
        IDasServiceBusEndpoint busEndpoint,
        IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
        ISystemClockService systemClockService,
        ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _busEndpoint = busEndpoint;
        _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
        _systemClockService = systemClockService;
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

        apprenticeship.RecalculatePayments(_systemClockService.Now);

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _apprenticeshipRepository.Update(apprenticeship);
        await _busEndpoint.Publish(@event);
    }
}