using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;

public static class IConfigurationExtensions
{
    public static bool NotAcceptanceTests(this IConfiguration configuration)
    {
        return !configuration!["EnvironmentName"]!.Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool NotLocalOrAcceptanceTests(this IConfiguration configuration)
    {
        return !configuration!["EnvironmentName"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) && configuration.NotAcceptanceTests();
    }
}
