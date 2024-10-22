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
        public async Task<Guid?> Get([ActivityTrigger] Learner input)
        {
            var response = await _queryHandler.Get(new GetApprenticeshipKeyQuery(input.Ukprn, input.Uln));
            return response.ApprenticeshipKey;
        }
    }
}