using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments
{
    public class GetDuePaymentsQueryHandler : IGetDuePaymentsQueryHandler
    {
        private IApprenticeshipRepository _repository;

        public GetDuePaymentsQueryHandler(IApprenticeshipRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDuePaymentsResponse> Get(GetDuePaymentsQuery command)
        {
            var apprenticeship = await _repository.Get(command.ApprenticeshipKey);
            var payments = apprenticeship.DuePayments(command.CollectionYear, command.CollectionPeriod);
            return new GetDuePaymentsResponse(payments.Select(x => new Payment(x.Key)));
        }
    }
}
