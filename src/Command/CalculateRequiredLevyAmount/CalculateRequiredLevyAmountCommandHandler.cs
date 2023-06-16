using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public class CalculateRequiredLevyAmountCommandHandler : ICalculateRequiredLevyAmountCommandHandler
{
    private readonly IMessageSession _messageSession;
    private readonly ILogger<CalculateRequiredLevyAmountCommandHandler> _logger;

    public CalculateRequiredLevyAmountCommandHandler(IMessageSession messageSession, ILogger<CalculateRequiredLevyAmountCommandHandler> logger)
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

        await _messageSession.Publish(@event);
    }
}