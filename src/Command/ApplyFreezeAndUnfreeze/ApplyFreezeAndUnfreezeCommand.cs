namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze
{
    public class ApplyFreezeAndUnfreezeCommand
    {
        public ApplyFreezeAndUnfreezeCommand(Guid apprenticeshipKey, short collectionYear, byte collectionPeriod)
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
