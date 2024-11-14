namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Dtos;

public class ResetSentForPaymentFlagForCollectionPeriodDto
{
    public byte CollectionPeriod { get; set; }
    public short CollectionYear { get; set; }
}