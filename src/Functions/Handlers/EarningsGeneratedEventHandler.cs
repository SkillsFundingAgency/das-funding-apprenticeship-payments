using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers;

public class EarningsGeneratedEventHandler(
    ICommandHandler<CalculateApprenticeshipPaymentsCommand> commandHandler,
    ILogger<EarningsGeneratedEventHandler> logger) : IHandleMessages<EarningsGeneratedEvent>
{
    public async Task Handle(EarningsGeneratedEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Handling EarningsGeneratedEvent");
        logger.LogInformation(JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true }));
        
        await commandHandler.Handle(new CalculateApprenticeshipPaymentsCommand(message));
    }
}

public class EarningsFunctions(
    ICommandHandler<CalculateApprenticeshipPaymentsCommand> calculateApprenticeshipPaymentsCommandHandler,
    ILogger<EarningsFunctions> logger)
{
    private readonly ILogger<EarningsFunctions> _logger = logger;

    [Function(nameof(EarningsGeneratedEventHttpTrigger))]
    public async Task EarningsGeneratedEventHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest request)
    {
        var earningsGeneratedEvent = new EarningsGeneratedEvent { ApprenticeshipKey = Guid.NewGuid() };
        await calculateApprenticeshipPaymentsCommandHandler.Handle(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
    }
}