using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using System.Text.Json;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;

public class CalculateApprenticeshipPaymentsCommandHandler : ICommandHandler<CalculateApprenticeshipPaymentsCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ISystemClockService _systemClockService;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;

    public CalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository,
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

    public async Task Handle(CalculateApprenticeshipPaymentsCommand command)
    {
        _logger.LogInformation($"ApprenticeshipKey: {command.EarningsGeneratedEvent.ApprenticeshipKey}");
        _logger.LogInformation($"Has DeliveryPeriods? {command.EarningsGeneratedEvent.DeliveryPeriods != null}");

       var apprenticeship = new Apprenticeship(command.EarningsGeneratedEvent);
        apprenticeship.CalculatePayments(_systemClockService.Now);
        _logger.LogInformation($"Publishing payments generated event for apprenticeship key {command.EarningsGeneratedEvent.ApprenticeshipKey}. Number of payments: {apprenticeship.Payments.Count}");

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _apprenticeshipRepository.Add(apprenticeship);
        await _busEndpoint.Publish(@event);
    }
}

