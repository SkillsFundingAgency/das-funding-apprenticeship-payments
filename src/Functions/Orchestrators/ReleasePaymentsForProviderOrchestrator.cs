using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using System.Collections.Generic;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;

public class ReleasePaymentsForProviderOrchestrator
{
    private readonly ILogger<ReleasePaymentsForProviderOrchestrator> _logger;

    public ReleasePaymentsForProviderOrchestrator(ILogger<ReleasePaymentsForProviderOrchestrator> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ReleasePaymentsForProviderOrchestrator))]
    public async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<ReleasePaymentsForProviderInput>();

        if (!context.IsReplaying)
            _logger.LogInformation("[ReleasePaymentsForProviderOrchestrator] Releasing payments for provider {ukprn} started", input.Ukprn);

        context.SetCustomStatus("GettingIlrSubmissions");
        var learnersInIlr = await context.CallActivityAsync<IEnumerable<Learner>>(nameof(GetLearnersInIlrSubmission), new GetLearnersInIlrSubmissionInput(input.Ukprn, input.CollectionDetails.CollectionYear, input.OrchestrationInstanceId));

        context.SetCustomStatus("ReleasingPaymentsForLearners");
        var releasePaymentsTasks = new List<Task>();
        foreach (var learner in learnersInIlr)
        {
            var releaseLearnerPaymentsTask = context.CallSubOrchestratorAsync(nameof(ReleasePaymentsForLearnerOrchestrator), new ReleasePaymentsForLearnerInput(input.CollectionDetails, learner, input.OrchestrationInstanceId));
            releasePaymentsTasks.Add(releaseLearnerPaymentsTask);
        }

        await Task.WhenAll(releasePaymentsTasks);
    }
}