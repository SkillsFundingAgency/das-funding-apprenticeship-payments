namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public class GetLearnersInIlrSubmissionInput
{
    public byte DeliveryPeriod { get; }
    public short AcademicYear { get; }

    public GetLearnersInIlrSubmissionInput(byte deliveryPeriod, short academicYear)
    {
        DeliveryPeriod = deliveryPeriod;
        AcademicYear = academicYear;
    }
}