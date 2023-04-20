using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class FinalisedOnProgammeLearningPaymentEventHandler : IHandleMessages<FinalisedOnProgammeLearningPaymentEvent>
{
    public static ConcurrentBag<FinalisedOnProgammeLearningPaymentEvent> ReceivedEvents { get; } = new();

    public Task Handle(FinalisedOnProgammeLearningPaymentEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}