using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class PaymentsFunctions
{
    [FunctionName(nameof(ReleasePaymentsEventServiceBusTrigger))]
    public async Task ReleasePaymentsEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        string instanceId = await starter.StartNewAsync(nameof(ReleasePaymentsOrchestrator), null, new CollectionDetails(releasePaymentsCommand.CollectionPeriod, releasePaymentsCommand.CollectionYear));

        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
    }

    [FunctionName(nameof(ReleasePaymentsHttpTrigger))]
    public static async Task<HttpResponseMessage> ReleasePaymentsHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "releasePayments/{collectionYear}/{collectionPeriod}")] HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        short collectionYear,
        byte collectionPeriod,
        ILogger log)
    {
        string instanceId = await starter.StartNewAsync(nameof(ReleasePaymentsOrchestrator), null, new CollectionDetails(collectionPeriod, collectionYear));

        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        return req.CreateResponse(HttpStatusCode.Accepted);
    }
}