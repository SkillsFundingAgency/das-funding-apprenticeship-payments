namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Dtos;

public class ReleasePaymentsDto
{
    public byte CollectionPeriod { get; set; }
    public short CollectionYear { get; set; }
    public short PreviousAcademicYear { get; set; }
    public DateTime HardCloseDate { get; set; }
}
