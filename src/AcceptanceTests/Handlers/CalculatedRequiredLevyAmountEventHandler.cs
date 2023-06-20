using NServiceBus;
using System.Collections.Concurrent;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class CalculatedRequiredLevyAmountHandler : IHandleMessages<CalculatedRequiredLevyAmount>
{
    public static ConcurrentBag<CalculatedRequiredLevyAmount> ReceivedEvents { get; } = new();

    public Task Handle(CalculatedRequiredLevyAmount message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}