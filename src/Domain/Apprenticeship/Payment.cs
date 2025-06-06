﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

[Table("Payment", Schema = "Domain")]
public class Payment
{
    private Payment() { }

    public Payment(Guid apprenticeshipKey, short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionPeriod, string fundingLineType, Guid earningsProfileId, string? paymentType)
    {
        Key = Guid.NewGuid();
        ApprenticeshipKey = apprenticeshipKey;
        AcademicYear = academicYear;
        DeliveryPeriod = deliveryPeriod;
        Amount = amount;
        CollectionYear = collectionYear;
        CollectionPeriod = collectionPeriod;
        FundingLineType = fundingLineType;
        SentForPayment = false;
        EarningsProfileId = earningsProfileId;
        PaymentType = paymentType;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Key { get; private set; }
    public Guid ApprenticeshipKey { get; private set; }
    public short AcademicYear { get; private set; }
    [Precision(15, 5)]
    public decimal Amount { get; private set; }
    public byte CollectionPeriod { get; private set; }
    public short CollectionYear { get; private set; }
    public byte DeliveryPeriod { get; private set; }
    public string FundingLineType { get; private set; }
    public bool SentForPayment { get; private set; }
    public Guid EarningsProfileId { get; private set; }
    public bool NotPaidDueToFreeze { get; private set; }
    public string? PaymentType { get; private set; }

    public void MarkAsNotPaid()
    {
        NotPaidDueToFreeze = true;
    }

    public void Unfreeze()
    {
        NotPaidDueToFreeze = false;
    }

    public void MarkAsSent(short collectionYear, byte collectionPeriod)
    {
        CollectionYear = collectionYear;
        CollectionPeriod = collectionPeriod;
        SentForPayment = true;
    }
}