namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.FreezePayments
{
    public interface IFreezePaymentsCommandHandler
    {
        Task Freeze(FreezePaymentsCommand command);
    }
}
