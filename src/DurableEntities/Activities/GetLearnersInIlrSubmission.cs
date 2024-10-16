using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class GetLearnersInIlrSubmission
    {
        public GetLearnersInIlrSubmission()
        {
        }

        [FunctionName(nameof(GetLearnersInIlrSubmission))]
        public async Task<IEnumerable<Learner>> Get([ActivityTrigger] object input)
        {
            throw new NotImplementedException();
        }
    }
}
