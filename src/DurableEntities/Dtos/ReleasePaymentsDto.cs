namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;

public class ReleasePaymentsDto
{
    public byte CollectionPeriod { get; set; }
    public short CollectionYear { get; set; }
    public short PreviousAcademicYear { get; set; }
    public DateTime HardCloseDate { get; set; }
}
