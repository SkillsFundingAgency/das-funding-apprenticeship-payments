using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class EarningsFunctions
{
    private readonly ICommandHandler<CalculateApprenticeshipPaymentsCommand> _calculateApprenticeshipPaymentsCommandHandler;
    private readonly ICommandHandler<RecalculateApprenticeshipPaymentsCommand> _recalculateApprenticeshipPaymentsCommandHandler;
    private readonly ILogger<EarningsFunctions> _logger;

    public EarningsFunctions(
        ICommandHandler<CalculateApprenticeshipPaymentsCommand> calculateApprenticeshipPaymentsCommandHandler,
        ICommandHandler<RecalculateApprenticeshipPaymentsCommand> recalculateApprenticeshipPaymentsCommandHandler,
        ILogger<EarningsFunctions> logger)
    {
        _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
        _recalculateApprenticeshipPaymentsCommandHandler = recalculateApprenticeshipPaymentsCommandHandler;
        _logger = logger;
    }

    [Function(nameof(EarningsGeneratedEventServiceBusTrigger))]
    public async Task EarningsGeneratedEventServiceBusTrigger(
        [ServiceBusTrigger(QueueNames.EarningsGenerated)] EarningsGeneratedEvent earningsGeneratedEvent)
    {
        await _calculateApprenticeshipPaymentsCommandHandler.Handle(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
    }

    [Function(nameof(EarningsGeneratedEventHttpTrigger))]
    public async Task EarningsGeneratedEventHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest request)
    {
        var earningsGeneratedEvent = new EarningsGeneratedEvent { ApprenticeshipKey = Guid.NewGuid() };
        await _calculateApprenticeshipPaymentsCommandHandler.Handle(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
    }

    [Function(nameof(EarningsRecalculatedEventServiceBusTrigger))]
    public async Task EarningsRecalculatedEventServiceBusTrigger(
        [ServiceBusTrigger(QueueNames.EarningsRecalculated)] ApprenticeshipEarningsRecalculatedEvent earningsRecalculatedEvent)
    {
        await _recalculateApprenticeshipPaymentsCommandHandler.Handle(
            new RecalculateApprenticeshipPaymentsCommand(earningsRecalculatedEvent.ApprenticeshipKey,
                earningsRecalculatedEvent.DeliveryPeriods.ToEarnings(earningsRecalculatedEvent.ApprenticeshipKey,
                    earningsRecalculatedEvent.EarningsProfileId), earningsRecalculatedEvent.StartDate, earningsRecalculatedEvent.PlannedEndDate, earningsRecalculatedEvent.AgeAtStartOfApprenticeship));
    }
}