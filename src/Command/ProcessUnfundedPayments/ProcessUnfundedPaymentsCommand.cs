using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments
{
    public class ProcessUnfundedPaymentsCommand
    {
        public ProcessUnfundedPaymentsCommand(byte collectionMonth, ApprenticeshipEntityModel model)
        {
            CollectionMonth = collectionMonth;
            Model = model;
        }

        public byte CollectionMonth { get; }
        public ApprenticeshipEntityModel Model { get; }
    }
}
