//using Microsoft.Azure.Functions.Worker;
//using Microsoft.DurableTask.Client;
//using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
//using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
//using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
//using SFA.DAS.Funding.ApprenticeshipPayments.Types;
//using System.Net.Http;

//namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

//public class PaymentsFunctions
//{
//    private readonly ILogger<PaymentsFunctions> _logger;

//    public PaymentsFunctions(ILogger<PaymentsFunctions> logger)
//    {
//        _logger = logger;
//    }

//    //[Function(nameof(ReleasePaymentsEventServiceBusTrigger))]
//    //public async Task ReleasePaymentsEventServiceBusTrigger(
//    //    [ServiceBusTrigger(QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
//    //    [DurableClient] DurableTaskClient client)
//    //{
//    //    var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
//    //        nameof(ReleasePaymentsOrchestrator),
//    //        input: new CollectionDetails(releasePaymentsCommand.CollectionPeriod, releasePaymentsCommand.CollectionYear)
//    //    );

//    //    _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");
//    //}

//    [Function(nameof(ReleasePaymentsHttpTrigger))]
//    public async Task ReleasePaymentsHttpTrigger(
//        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "releasePayments/{collectionYear}/{collectionPeriod}")] HttpRequestMessage req,
//        [DurableClient] DurableTaskClient client,
//        short collectionYear,
//        byte collectionPeriod)
//    {
//        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
//            nameof(ReleasePaymentsOrchestrator),
//            input: new CollectionDetails(collectionPeriod, collectionYear)
//        );

//        _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");
//    }
//}