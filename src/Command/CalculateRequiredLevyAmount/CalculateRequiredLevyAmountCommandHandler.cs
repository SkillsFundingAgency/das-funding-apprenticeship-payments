using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Text.Json;
using NServiceBus;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public class CalculateRequiredLevyAmountCommandHandler : ICalculateRequiredLevyAmountCommandHandler
{
    private readonly IPaymentsV2ServiceBusEndpoint _busEndpoint;
    private readonly IMessageSession _messageSession;
    private readonly ILogger<CalculateRequiredLevyAmountCommandHandler> _logger;

    public CalculateRequiredLevyAmountCommandHandler(IMessageSession messageSession
        , ILogger<CalculateRequiredLevyAmountCommandHandler> logger)
    {
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task Process(CalculateRequiredLevyAmountCommand command)
    {
        var @event = command.MapToCalculatedRequiredLevyAmountEvent();

        _logger.LogInformation(
            "Apprenticeship Key: {ApprenticeshipKey} - Publishing {event} for CollectionPeriod: {Period}/{AcademicYear}",
            command.Data.ApprenticeshipKey,
            nameof(CalculatedRequiredLevyAmount),
            @event.CollectionPeriod.Period,
        @event.CollectionPeriod.AcademicYear);


        _logger.LogInformation("ApprenticeshipKey: {0} Publishing CalculatedRequiredLevyAmount: {1}",
            command.Data.ApprenticeshipKey,
            JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = true }));

        await _messageSession.Send(@event);
    }
}