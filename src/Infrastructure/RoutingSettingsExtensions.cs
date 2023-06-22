﻿using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure
{
    public static class RoutingSettingsExtensions
    {
        public static void AddRouting(this RoutingSettings settings)
        {
            settings.RouteToEndpoint(typeof(CalculatedRequiredLevyAmount2), QueueNames.CalculatedRequiredLevyAmount);
        }
    }
}
