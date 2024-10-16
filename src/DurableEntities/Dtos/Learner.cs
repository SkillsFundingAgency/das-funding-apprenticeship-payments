namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos
{
    public class Learner
    {
        public long Ukprn { get; }
        public long Uln { get; }
        public string LearnerRef { get; }

        public Learner(long ukprn, long uln, string learnerRef)
        {
            Ukprn = ukprn;
            Uln = uln;
            LearnerRef = learnerRef;
        }
    }
}
