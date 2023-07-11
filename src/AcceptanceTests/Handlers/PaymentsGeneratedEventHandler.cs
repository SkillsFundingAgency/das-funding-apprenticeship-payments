using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class PaymentsGeneratedEventHandler : IHandleMessages<PaymentsGeneratedEvent>
{
    public static ConcurrentBag<PaymentsGeneratedEvent> ReceivedEvents { get; } = new();

    public Task Handle(PaymentsGeneratedEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}