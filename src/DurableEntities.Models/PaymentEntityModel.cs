﻿namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models
{
    public class PaymentEntityModel
    {
        public short AcademicYear { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
        public short CollectionYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public bool SentForPayment { get; set; }
        public string FundingLineType { get; set; } = null!;
        public Guid EarningsProfileId { get; set; }
        public bool NotPaidDueToFreeze { get; set; }
    }
}
