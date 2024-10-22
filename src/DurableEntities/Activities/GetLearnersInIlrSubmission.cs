using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;
using Learner = SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos.Learner;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class GetLearnersInIlrSubmission
    {
        private readonly IGetLearnersInILRQueryHandler _queryHandler;

        public GetLearnersInIlrSubmission(IGetLearnersInILRQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [FunctionName(nameof(GetLearnersInIlrSubmission))]
        public async Task<IEnumerable<Learner>> Get([ActivityTrigger] long ukprn)
        {
            var learners = await _queryHandler.Get(new GetLearnersInILRQuery(ukprn));
            return learners.Learners.Select(x => new Learner(ukprn, x.Uln, x.LearnerRef));
        }
    }
}
