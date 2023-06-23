using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure
{
    public static class RoutingSettingsExtensions
    {
        public static RoutingSettings AddRouting(this RoutingSettings settings)
        {
            settings.RouteToEndpoint(typeof(CalculatedRequiredLevyAmount), QueueNames.CalculatedRequiredLevyAmount);

            return settings;
        }
    }
}
