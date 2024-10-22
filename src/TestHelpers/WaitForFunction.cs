using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers
{
    public static class WaitForFunction
    {
        [FunctionName(nameof(WaitForFunction))]
        [NoAutomaticTrigger]
        public static async Task Run([DurableClient] IDurableOrchestrationClient client, string name, TimeSpan? timeout)
        {
            using var cts = new CancellationTokenSource();
            if (timeout != null)
            {
                cts.CancelAfter(timeout.Value);
            }

            await client.Wait(status => status.All(x => OrchestrationsComplete(name, x)), cts.Token);
        }

        private static bool OrchestrationsComplete(string orchestratorName, DurableOrchestrationStatus orchestrationStatus)
        {
            return orchestrationStatus.Name != orchestratorName;
        }
    }
}
