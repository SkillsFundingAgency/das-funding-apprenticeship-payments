namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain;

/// <summary>
/// This defines both Instalment (within Earning) and Payment (within Payment) types.
/// </summary>
public static class InstalmentTypes
{
    public const string OnProgramme = "OnProgramme";
    public const string ProviderIncentive = "ProviderIncentive";
    public const string EmployerIncentive = "EmployerIncentive";
    public const string MathsAndEnglish = "MathsAndEnglish";

    public static readonly List<string> AllTypes = new()
    {
        OnProgramme,
        ProviderIncentive,
        EmployerIncentive
    };

    /// <summary>
    /// Returns a normalised value for the InstalmentType. 
    /// This is because the InstalmentType is not always set in the source data and should default to OnProgramme.
    /// </summary>
    public static string ToInstalmentType(this string? source)
    {
        if(string.IsNullOrWhiteSpace(source))
        {
            return OnProgramme; // At the point InstalmentType was introduced, all existing records were OnProgramme
        }
        return source;
    }
}
