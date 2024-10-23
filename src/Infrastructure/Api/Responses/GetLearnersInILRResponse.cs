namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;

public class Learner
{
    public long Uln { get; set; }
    public string LearnerRefNumber { get; set; }
}

public class GetLearnersInILRResponse
{
    public List<Learner> Learners { get; set; }
}
