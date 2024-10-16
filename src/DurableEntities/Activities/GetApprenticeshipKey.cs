using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class GetApprenticeshipKey
    {
        private readonly IGetApprenticeshipKeyQueryHandler _queryHandler;

        public GetApprenticeshipKey(IGetApprenticeshipKeyQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [FunctionName(nameof(GetApprenticeshipKey))]
        public async Task<Guid?> Get([ActivityTrigger] object input)
        {
            var learner = (Learner)input;
            var response = await _queryHandler.Get(new GetApprenticeshipKeyQuery(learner.Ukprn, learner.Uln));
            return response.ApprenticeshipKey;
        }
    }
}