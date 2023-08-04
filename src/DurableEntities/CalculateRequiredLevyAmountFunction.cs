using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

public class CalculateRequiredLevyAmountFunction
{
    private readonly ICalculateRequiredLevyAmountCommandHandler _commandHandler;

    public CalculateRequiredLevyAmountFunction(ICalculateRequiredLevyAmountCommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [FunctionName("CalculateRequiredLevyAmountFunction_Http")]
    public async Task EarningsGeneratedEventHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest request,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        Fixture fixture = new();
        var data = fixture.
            Build<FinalisedOnProgammeLearningPaymentEvent>()
            .With(x => x.CourseCode, "000000")
            .Create();

        await _commandHandler.Process(new CalculateRequiredLevyAmountCommand(
            data));

    }

    [FunctionName(nameof(CalculateRequiredLevyAmountFunction))]
    public async Task Run(
        [NServiceBusTrigger(Endpoint = QueueNames.FinalisedOnProgammeLearningPayment)] FinalisedOnProgammeLearningPaymentEvent @event,
            
        ILogger log)
    {
        log.LogInformation(
            "Triggered {0} function for ApprenticeshipKey: {1}", nameof(CalculateRequiredLevyAmountFunction), @event.ApprenticeshipKey);

        log.LogInformation("ApprenticeshipKey: {0} Received FinalisedOnProgammeLearningPaymentEvent: {1}",
            @event.ApprenticeshipKey,
            JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = true }));

        await _commandHandler.Process(new CalculateRequiredLevyAmountCommand(@event));
    }
}