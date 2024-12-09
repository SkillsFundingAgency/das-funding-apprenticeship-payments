using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public class CalculateRequiredLevyAmountCommandHandler : ICommandHandler<CalculateRequiredLevyAmountCommand>
{
    private readonly IPaymentsV2ServiceBusEndpoint _busEndpoint;
    private readonly ILogger<CalculateRequiredLevyAmountCommandHandler> _logger;

    public CalculateRequiredLevyAmountCommandHandler(IPaymentsV2ServiceBusEndpoint busEndpoint, ILogger<CalculateRequiredLevyAmountCommandHandler> logger)
    {
        _busEndpoint = busEndpoint;
        _logger = logger;
    }

    public async Task Handle(CalculateRequiredLevyAmountCommand command)
    {
        var @event = command.MapToCalculateOnProgrammePaymentEvent();

        _logger.LogInformation(
            "Apprenticeship Key: {ApprenticeshipKey} - Publishing {event} for CollectionPeriod: {Period}/{AcademicYear}",
            command.Data.ApprenticeshipKey,
            nameof(CalculateOnProgrammePayment),
            @event.CollectionPeriod.Period,
        @event.CollectionPeriod.AcademicYear);


        _logger.LogInformation("ApprenticeshipKey: {0} Publishing CalculatedRequiredLevyAmount: {1}",
            command.Data.ApprenticeshipKey,
            @event.SerialiseForLogging());

        await _busEndpoint.Send(@event);
    }
}