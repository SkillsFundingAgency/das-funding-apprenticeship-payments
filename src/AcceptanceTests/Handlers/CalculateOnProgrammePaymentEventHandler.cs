using NServiceBus;
using System.Collections.Concurrent;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class CalculateOnProgrammePaymentEventHandler : IHandleMessages<CalculateOnProgrammePayment>
{
    public static ConcurrentBag<CalculateOnProgrammePayment> ReceivedEvents { get; } = new();

    public Task Handle(CalculateOnProgrammePayment message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}