namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR
{
    public class Learner
    {
        public long Uln { get; }
        public string LearnerRef { get; }

        public Learner(long uln, string learnerRef)
        {
            Uln = uln;
            LearnerRef = learnerRef;
        }
    }

    public class GetLearnersInILRQueryResponse
    {
        public GetLearnersInILRQueryResponse(IEnumerable<Learner> learners)
        {
            Learners = learners;
        }

        public IEnumerable<Learner> Learners { get; }
    }
}
