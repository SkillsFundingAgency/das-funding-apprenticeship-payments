﻿using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments
{
    public class ProcessUnfundedPaymentsCommand
    {
        public ProcessUnfundedPaymentsCommand(byte collectionPeriod, short collectionYear, ApprenticeshipEntityModel model)
        {
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
            Model = model;
        }

        public byte CollectionPeriod { get; }
        public short CollectionYear { get; set; }
        public ApprenticeshipEntityModel Model { get; }
    }
}
