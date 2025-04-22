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
}
