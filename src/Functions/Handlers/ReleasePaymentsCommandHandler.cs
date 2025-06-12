using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
using System.Net;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers
{
    public class PaymentsFunctions(ILogger<PaymentsFunctions> logger)
    {
        [Function(nameof(ReleasePaymentsHttpTrigger))]
        public async Task<HttpResponseData> ReleasePaymentsHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "releasePayments/{collectionYear}/{collectionPeriod}")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            short collectionYear,
            byte collectionPeriod)
        {
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ReleasePaymentsOrchestrator),
                input: new CollectionDetails(collectionPeriod, collectionYear)
            );

            logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("InstanceId", instanceId);
            return response;
        }
    }
}
