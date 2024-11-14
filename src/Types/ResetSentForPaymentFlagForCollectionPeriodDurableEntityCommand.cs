namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class ResetSentForPaymentFlagForCollectionPeriodDurableEntityCommand
{
    public byte CollectionPeriod { get; set; }
    public short CollectionYear { get; set; }
}