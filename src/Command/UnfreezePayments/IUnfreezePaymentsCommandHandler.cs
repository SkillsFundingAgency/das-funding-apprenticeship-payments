namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments
{
    public interface IUnfreezePaymentsCommandHandler
    {
        Task Unfreeze(UnfreezePaymentsCommand command);
    }
}
