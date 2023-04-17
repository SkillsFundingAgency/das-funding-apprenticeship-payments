using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments
{
    public class CalculateApprenticeshipPaymentsCommand
    {
        public CalculateApprenticeshipPaymentsCommand(ApprenticeshipEntityModel apprenticeshipEntity)
        {
            ApprenticeshipEntity = apprenticeshipEntity;
        }

        public ApprenticeshipEntityModel ApprenticeshipEntity { get; }
    }
}
