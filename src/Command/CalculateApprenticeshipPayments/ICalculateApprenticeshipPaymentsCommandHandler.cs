using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments
{
    public interface ICalculateApprenticeshipPaymentsCommandHandler
    {
        Task<Apprenticeship> Calculate(CalculateApprenticeshipPaymentsCommand command);
    }
}
