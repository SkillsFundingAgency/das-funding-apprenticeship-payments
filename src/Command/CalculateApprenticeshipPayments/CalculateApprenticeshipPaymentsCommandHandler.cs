using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;

public class CalculateApprenticeshipPaymentsCommandHandler : ICalculateApprenticeshipPaymentsCommandHandler
{
    private readonly IApprenticeshipFactory _apprenticeshipFactory;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;
    private readonly ISystemClockService _systemClockService;

    public CalculateApprenticeshipPaymentsCommandHandler(
        IApprenticeshipFactory apprenticeshipFactory,
        IDasServiceBusEndpoint busEndpoint,
        IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
        ISystemClockService systemClockService,
        ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        _apprenticeshipFactory = apprenticeshipFactory;
        _busEndpoint = busEndpoint;
        _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
        _systemClockService = systemClockService;
        _logger = logger;
    }

    public async Task<Apprenticeship> Calculate(CalculateApprenticeshipPaymentsCommand command)
    {
        var apprenticeship = _apprenticeshipFactory.CreateNew(command.ApprenticeshipEntity);
        apprenticeship.CalculatePayments(_systemClockService.Now);
        command.ApprenticeshipEntity.Payments = MapPaymentsToModel(apprenticeship.Payments);
        _logger.LogInformation($"Publishing payments generated event for apprenticeship key {command.ApprenticeshipEntity.ApprenticeshipKey}. Number of payments: {command.ApprenticeshipEntity.Payments.Count}");

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _busEndpoint.Publish(@event);
        return apprenticeship;
    }

    private static List<PaymentEntityModel> MapPaymentsToModel(IReadOnlyCollection<Payment> apprenticeshipPayments)
    {
        return apprenticeshipPayments.Select(x => new PaymentEntityModel
        {
            CollectionYear = x.CollectionYear,
            AcademicYear = x.AcademicYear,
            Amount = x.Amount,
            DeliveryPeriod = x.DeliveryPeriod,
            CollectionPeriod = x.CollectionPeriod,
            SentForPayment = x.SentForPayment,
            FundingLineType = x.FundingLineType,
            EarningsProfileId = x.EarningsProfileId
        }).ToList();
    }
}
