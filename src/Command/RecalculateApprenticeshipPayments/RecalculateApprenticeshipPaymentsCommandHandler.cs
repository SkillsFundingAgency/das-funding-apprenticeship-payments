using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public class RecalculateApprenticeshipPaymentsCommandHandler : IRecalculateApprenticeshipPaymentsCommandHandler
{
    private readonly IApprenticeshipFactory _apprenticeshipFactory;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;

    public RecalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipFactory apprenticeshipFactory,
        IDasServiceBusEndpoint busEndpoint,
        IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
        ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        _apprenticeshipFactory = apprenticeshipFactory;
        _busEndpoint = busEndpoint;
        _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
        _logger = logger;
    }

    public async Task<Apprenticeship> Recalculate(RecalculateApprenticeshipPaymentsCommand command)
    {
        var apprenticeship = _apprenticeshipFactory.LoadExisting(command.ApprenticeshipEntity);

        apprenticeship.ClearEarnings();
        
        foreach (var earning in command.NewEarnings)
        {
            apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth, earning.FundingLineType);
        }

        apprenticeship.RecalculatePayments(DateTime.Now);

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _busEndpoint.Publish(@event);
        return apprenticeship;
    }
}