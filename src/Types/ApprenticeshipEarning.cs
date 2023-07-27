using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class ApprenticeshipEarning : IEvent
{
    public long FundingPeriodId { get; set; }
    public byte DeliveryPeriod { get; set; }
    public Guid ApprenticeshipEarningsId { get; set; }
    public decimal DeliveryPeriodAmount { get; set; }
    public short NumberOfInstalments { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public decimal GovernmentContributionPercentage { get; set; }
    public long ProviderIdentifier { get; set; }
    public long Uln { get; set; }
    public string? FundingLineType { get; set; }
    public DateTime StartDate { get; set; }
}