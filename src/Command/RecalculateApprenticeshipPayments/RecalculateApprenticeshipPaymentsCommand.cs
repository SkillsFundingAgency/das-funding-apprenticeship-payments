using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments
{
    public class RecalculateApprenticeshipPaymentsCommand
    {
        public RecalculateApprenticeshipPaymentsCommand(Guid apprenticeshipKey, List<Earning> newEarnings)
        {
            ApprenticeshipKey = apprenticeshipKey;
            NewEarnings = newEarnings;
        }

        public Guid ApprenticeshipKey { get; }
        public List<Earning> NewEarnings { get; set; }
    }
}
