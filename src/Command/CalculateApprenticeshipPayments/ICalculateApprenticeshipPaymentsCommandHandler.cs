namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments
{
    public interface ICalculateApprenticeshipPaymentsCommandHandler
    {
        Task Calculate(CalculateApprenticeshipPaymentsCommand command);
    }
}
