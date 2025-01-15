using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class EarningsFunctions
{
    private readonly ICommandHandler<CalculateApprenticeshipPaymentsCommand> _calculateApprenticeshipPaymentsCommandHandler;
    private readonly ICommandHandler<RecalculateApprenticeshipPaymentsCommand> _recalculateApprenticeshipPaymentsCommandHandler;

    public EarningsFunctions(ICommandHandler<CalculateApprenticeshipPaymentsCommand> calculateApprenticeshipPaymentsCommandHandler, ICommandHandler<RecalculateApprenticeshipPaymentsCommand> recalculateApprenticeshipPaymentsCommandHandler)
    {
        _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
        _recalculateApprenticeshipPaymentsCommandHandler = recalculateApprenticeshipPaymentsCommandHandler;
    }

    [FunctionName(nameof(EarningsGeneratedEventServiceBusTrigger))]
    public async Task EarningsGeneratedEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.EarningsGenerated)] EarningsGeneratedEvent earningsGeneratedEvent,
        ILogger log)
    {
        await _calculateApprenticeshipPaymentsCommandHandler.Handle(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
    }

    [FunctionName(nameof(EarningsGeneratedEventHttpTrigger))]
    public async Task EarningsGeneratedEventHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest request,
        ILogger log)
    {
        var earningsGeneratedEvent = new EarningsGeneratedEvent { ApprenticeshipKey = Guid.NewGuid() };
        await _calculateApprenticeshipPaymentsCommandHandler.Handle(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
    }

    [FunctionName(nameof(EarningsRecalculatedEventServiceBusTrigger))]
    public async Task EarningsRecalculatedEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.EarningsRecalculated)] ApprenticeshipEarningsRecalculatedEvent earningsRecalculatedEvent,
        ILogger log)
    {
        await _recalculateApprenticeshipPaymentsCommandHandler.Handle(
            new RecalculateApprenticeshipPaymentsCommand(earningsRecalculatedEvent.ApprenticeshipKey,
                earningsRecalculatedEvent.DeliveryPeriods.ToEarnings(earningsRecalculatedEvent.ApprenticeshipKey,
                    earningsRecalculatedEvent.EarningsProfileId), earningsRecalculatedEvent.StartDate, earningsRecalculatedEvent.PlannedEndDate, earningsRecalculatedEvent.AgeAtStartOfApprenticeship));
    }
}