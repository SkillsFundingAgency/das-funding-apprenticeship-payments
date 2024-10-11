namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments
{
    public class ProcessUnfundedPaymentsCommand
    {
        public ProcessUnfundedPaymentsCommand(byte collectionPeriod, short collectionYear, Guid apprenticeshipKey, short previousAcademicYear, DateTime hardCloseDate,)
        {
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
            ApprenticeshipKey = apprenticeshipKey;
            PreviousAcademicYear = previousAcademicYear;
            HardCloseDate = hardCloseDate;
        }

        public byte CollectionPeriod { get; }
        public short CollectionYear { get; set; }
        public Guid ApprenticeshipKey { get; }
        public short PreviousAcademicYear { get; set; }
        public DateTime HardCloseDate { get; set; }
    }
}
