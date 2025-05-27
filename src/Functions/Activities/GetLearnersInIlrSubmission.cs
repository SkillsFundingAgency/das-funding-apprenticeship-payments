using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Query;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;
using Learner = SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos.Learner;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;

public class GetLearnersInIlrSubmission
{
    private readonly IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery> _queryHandler;
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetLearnersInIlrSubmission(IQueryHandler<GetLearnersInILRQueryResponse, GetLearnersInILRQuery> queryHandler, IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _queryHandler = queryHandler;
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    [Function(nameof(GetLearnersInIlrSubmission))]
    public async Task<IEnumerable<Learner>> Get([ActivityTrigger] GetLearnersInIlrSubmissionInput input)
    {
        var apprenticeships =
            await _apprenticeshipQueryRepository.GetApprenticeshipsWithDuePayments(input.AcademicYear,
                input.DeliveryPeriod);

        return apprenticeships.Select(x => new Learner(x.Ukprn, x.Uln, x.LearnerReference));
    }
}