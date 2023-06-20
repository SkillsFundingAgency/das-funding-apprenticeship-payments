using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
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