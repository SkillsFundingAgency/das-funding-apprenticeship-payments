using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers
{
    public class EarningsGeneratedEventHandler(ILogger<EarningsGeneratedEventHandler> logger) : IHandleMessages<EarningsGeneratedEvent>
    {
        public Task Handle(EarningsGeneratedEvent message, IMessageHandlerContext context)
        {
            logger.LogInformation("EarningsGeneratedEvent handled");
            return Task.CompletedTask;
        }
    }
}
