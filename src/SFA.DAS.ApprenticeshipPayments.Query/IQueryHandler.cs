namespace SFA.DAS.Funding.ApprenticeshipPayments.Query
{
    public interface IQueryHandler<TOutput, TInput>
    {
        public Task<TOutput> Get(TInput query);
    }
}
