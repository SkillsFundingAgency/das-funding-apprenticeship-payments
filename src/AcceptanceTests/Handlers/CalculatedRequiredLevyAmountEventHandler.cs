﻿using NServiceBus;
using System.Collections.Concurrent;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;

public class CalculatedRequiredLevyAmountHandler : IHandleMessages<CalculatedRequiredLevyAmount2>
{
    public static ConcurrentBag<CalculatedRequiredLevyAmount2> ReceivedEvents { get; } = new();

    public Task Handle(CalculatedRequiredLevyAmount2 message, IMessageHandlerContext context)
    {
        ReceivedEvents.Add(message);
        return Task.CompletedTask;
    }
}