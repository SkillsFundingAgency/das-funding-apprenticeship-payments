using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;
using Learner = SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos.Learner;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class GetLearnersInIlrSubmission
{
    private readonly IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery> _queryHandler;

    public GetLearnersInIlrSubmission(IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    [FunctionName(nameof(GetLearnersInIlrSubmission))]
    public async Task<IEnumerable<Learner>> Get([ActivityTrigger] GetLearnersInIlrSubmissionInput input)
    {
        var learners = await _queryHandler.Get(new GetLearnersInILRQuery(input.Ukprn, input.AcademicYear));
        return learners.Learners.Select(x => new Learner(input.Ukprn, x.Uln, x.LearnerRef));
    }
}