using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class CalculatedOnProgrammeFundingEventHandler : IHandleMessages<CalculatedOnProgrammeFundingEvent>
{
    public static ConcurrentBag<CalculatedOnProgrammeFundingEvent> ReceivedEvents { get; } = new();

    public Task Handle(CalculatedOnProgrammeFundingEvent message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}