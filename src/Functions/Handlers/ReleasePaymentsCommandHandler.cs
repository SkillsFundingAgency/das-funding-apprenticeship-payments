﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using System.Net.Http;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers
{
    public class ReleasePaymentsCommandHandler(DurableTaskClient client, ILogger<ReleasePaymentsCommand> logger) : IHandleMessages<ReleasePaymentsCommand>
    {
        public async Task Handle(ReleasePaymentsCommand message, IMessageHandlerContext context)
        {
            logger.LogInformation("Handling ReleasePaymentCommand");

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ReleasePaymentsOrchestrator),
                input: new CollectionDetails(message.CollectionPeriod, message.CollectionYear));

            logger.LogInformation($"Scheduled new orchestration instance Id {instanceId}");
        }
    }
}

public class PaymentsFunctions
{
    private readonly ILogger<PaymentsFunctions> _logger;

    public PaymentsFunctions(ILogger<PaymentsFunctions> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ReleasePaymentsHttpTrigger))]
    public async Task ReleasePaymentsHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "releasePayments/{collectionYear}/{collectionPeriod}")] HttpRequestMessage req,
        [DurableClient] DurableTaskClient client,
        short collectionYear,
        byte collectionPeriod)
    {
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ReleasePaymentsOrchestrator),
            input: new CollectionDetails(collectionPeriod, collectionYear)
        );

        _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");
    }
}
