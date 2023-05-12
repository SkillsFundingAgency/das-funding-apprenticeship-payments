using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments
{
    public class ProcessUnfundedPaymentsCommand
    {
        public ProcessUnfundedPaymentsCommand(byte collectionPeriod, ApprenticeshipEntityModel model)
        {
            CollectionPeriod = collectionPeriod;
            Model = model;
        }

        public byte CollectionPeriod { get; }
        public ApprenticeshipEntityModel Model { get; }
    }
}
