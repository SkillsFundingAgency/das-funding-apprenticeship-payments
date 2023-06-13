namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class ApprenticeshipEarning
{
    public string FundingPeriodId { get; set; }
    public string DeliveryPeriod { get; set; }
    public Guid ApprenticeshipEarningsId { get; set; }
    public decimal DeliveryPeriodAmount { get; set; }
    public int NumberOfInstalments { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public decimal GovernmentContributionPercentage { get; set; }
    public long ProviderIdentifier { get; set; }
    public long Uln { get; set; }
    public string FundingLineType { get; set; }
}