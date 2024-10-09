using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments
{
    public class GetApprenticeshipsWithDuePaymentsQueryHandler : IGetApprenticeshipsWithDuePaymentsQueryHandler
    {
        private IApprenticeshipQueryRepository _repository;

        public GetApprenticeshipsWithDuePaymentsQueryHandler(IApprenticeshipQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetApprenticeshipsWithDuePaymentsResponse> Get(GetApprenticeshipsWithDuePaymentsQuery command)
        {
            var apprenticeships = await _repository.GetWithDuePayments(command.CollectionYear, command.CollectionPeriod);
            return new GetApprenticeshipsWithDuePaymentsResponse(apprenticeships.Select(x => new Apprenticeship(x)));
        }
    }
}
