namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments
{
    public class ProcessUnfundedPaymentsCommand
    {
        public ProcessUnfundedPaymentsCommand(byte collectionPeriod, short collectionYear, Guid apprenticeshipKey)
        {
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
            ApprenticeshipKey = apprenticeshipKey;
        }

        public byte CollectionPeriod { get; }
        public short CollectionYear { get; set; }
        public Guid ApprenticeshipKey { get; }
    }
}
