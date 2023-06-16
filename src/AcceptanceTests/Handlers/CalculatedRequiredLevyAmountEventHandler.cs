using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Concurrent;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class CalculatedRequiredLevyAmountEventHandler : IHandleMessages<CalculatedRequiredLevyAmountEvent>
{
    public static ConcurrentBag<CalculatedRequiredLevyAmountEvent> ReceivedEvents { get; } = new();

    public Task Handle(CalculatedRequiredLevyAmountEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}