using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;
using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;

public class ReleasePaymentsForLearnerOrchestrator
{
    private readonly ILogger<ReleasePaymentsForLearnerOrchestrator> _logger;

    public ReleasePaymentsForLearnerOrchestrator(ILogger<ReleasePaymentsForLearnerOrchestrator> logger)
    {
        _logger = logger;
    }

    [FunctionName(nameof(ReleasePaymentsForLearnerOrchestrator))]
    public async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var input = context.GetInput<ReleasePaymentsForLearnerInput>();

        if (!context.IsReplaying)
            _logger.LogInformation("[ReleasePaymentsForLearnerOrchestrator] Releasing payments for learner {LearnerRef} started", input.Learner.LearnerRef);

        context.SetCustomStatus("GettingApprenticeshipKey");

        var apprenticeshipKey = await context.CallActivityAsync<Guid?>(nameof(GetApprenticeshipKey), input.Learner);

        if (!apprenticeshipKey.HasValue)
        {
            _logger.LogInformation("[ReleasePaymentsForLearnerOrchestrator] No apprenticeship found for learner {LearnerRef}", input.Learner.LearnerRef);
            return;
        }

        context.SetCustomStatus("SettingLearnerReference");

        await context.CallActivityAsync(nameof(SetLearnerReference), new SetLearnerReferenceInput(apprenticeshipKey.Value, input.Learner.LearnerRef));

        context.SetCustomStatus("ApplyingFreeAndUnfreeze");

        await context.CallActivityAsync(nameof(ApplyFreezeAndUnfreeze), new ApplyFreezeAndUnfreezeInput(input.CollectionDetails, apprenticeshipKey.Value));

        context.SetCustomStatus("GettingDuePayments");

        var duePayments = await context.CallActivityAsync<IEnumerable<Guid>>(nameof(GetDuePayments), new GetDuePaymentsInput(input.CollectionDetails, apprenticeshipKey.Value));

        context.SetCustomStatus("ReleasingPayments");
        var releasePaymentsTasks = new List<Task>();
        foreach (var payment in duePayments)
        {
            var releasePaymentsTask = context.CallActivityAsync(nameof(ReleasePayment), new ReleasePaymentInput(apprenticeshipKey.Value, payment, input.CollectionDetails));
            releasePaymentsTasks.Add(releasePaymentsTask);
        }

        await Task.WhenAll(releasePaymentsTasks);
    }
}