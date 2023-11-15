using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public interface IRecalculateApprenticeshipPaymentsCommandHandler
{
    Task<Apprenticeship> Recalculate(RecalculateApprenticeshipPaymentsCommand command);
}