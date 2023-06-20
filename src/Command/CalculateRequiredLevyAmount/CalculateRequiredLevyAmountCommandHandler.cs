using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public class CalculateRequiredLevyAmountCommandHandler : ICalculateRequiredLevyAmountCommandHandler
{
    private readonly IPaymentsV2ServiceBusEndpoint _busEndpoint;
    private readonly ILogger<CalculateRequiredLevyAmountCommandHandler> _logger;

    public CalculateRequiredLevyAmountCommandHandler(IPaymentsV2ServiceBusEndpoint busEndpoint, ILogger<CalculateRequiredLevyAmountCommandHandler> logger)
    {
        _busEndpoint = busEndpoint;
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

        await _busEndpoint.Publish(@event);
    }
}