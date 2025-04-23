namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain;

/// <summary>
/// This defines both Instalment (within Earning) and Payment (within Payment) types.
/// </summary>
public static class InstalmentTypes
{
    public const string OnProgramme = "OnProgramme";
    public const string ProviderIncentive = "ProviderIncentive";
    public const string EmployerIncentive = "EmployerIncentive";

    public static readonly List<string> AllTypes = new()
    {
        OnProgramme,
        ProviderIncentive,
        EmployerIncentive
    };

    public static bool IsOnProgramme(this string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return true; // At the point InstalmentType was introduced, all existing records were OnProgramme
        }

        return type == OnProgramme;
    }

    public static bool IsIncentive(this string? type)
    {
        return type == ProviderIncentive || type == EmployerIncentive;
    }
}
