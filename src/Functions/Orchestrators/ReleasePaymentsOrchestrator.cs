using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;

public class ReleasePaymentsOrchestrator
{
    private readonly ILogger<ReleasePaymentsOrchestrator> _logger;

    public ReleasePaymentsOrchestrator(ILogger<ReleasePaymentsOrchestrator> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ReleasePaymentsOrchestrator))]
    public async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<CollectionDetails>();

        if (!context.IsReplaying)
            _logger.LogInformation("[ReleasePaymentsOrchestrator] Release Payment process started");

        context.SetCustomStatus("GettingProviders");

        var providers = await context.CallActivityAsync<IEnumerable<long>>(nameof(GetProviders), null);

        context.SetCustomStatus("ReleasingPaymentsForProviders");
        var releasePaymentsTasks = new List<Task>();
        foreach (var provider in providers)
        {
            var releaseProviderPaymentsTask = context.CallSubOrchestratorAsync(nameof(ReleasePaymentsForProviderOrchestrator), new ReleasePaymentsForProviderInput(input, provider, context.InstanceId));
            releasePaymentsTasks.Add(releaseProviderPaymentsTask);
        }

        await Task.WhenAll(releasePaymentsTasks);
    }
}