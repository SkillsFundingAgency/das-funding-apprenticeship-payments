using System.Diagnostics.CodeAnalysis;
using NServiceBus;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

[ExcludeFromCodeCoverage]
public static class RoutingSettingsExtensions
{
    public static RoutingSettings AddRouting(this RoutingSettings settings)
    {
        settings.RouteToEndpoint(typeof(CalculateOnProgrammePayment), QueueNames.CalculatedRequiredLevyAmount);

        return settings;
    }
}
