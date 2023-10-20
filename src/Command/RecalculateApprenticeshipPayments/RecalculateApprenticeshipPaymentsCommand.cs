using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments
{
    public class RecalculateApprenticeshipPaymentsCommand
    {
        public RecalculateApprenticeshipPaymentsCommand(ApprenticeshipEntityModel apprenticeshipEntity, List<Earning> newEarnings)
        {
            ApprenticeshipEntity = apprenticeshipEntity;
            NewEarnings = newEarnings;
        }

        public ApprenticeshipEntityModel ApprenticeshipEntity { get; }
        public List<Earning> NewEarnings { get; set; }
    }
}
