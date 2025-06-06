﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

[Table("Earning", Schema = "Domain")]
public class Earning
{
    private Earning() { }

    public Earning(Guid apprenticeshipKey, short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth, string fundingLineType, Guid earningsProfileId, string instalmentType)
    {
        Key = Guid.NewGuid();
        ApprenticeshipKey = apprenticeshipKey;
        AcademicYear = academicYear;
        DeliveryPeriod = deliveryPeriod;
        Amount = amount;
        CollectionMonth = collectionMonth;
        FundingLineType = fundingLineType;
        CollectionYear = collectionYear;
        EarningsProfileId = earningsProfileId;
        InstalmentType = instalmentType;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Key { get; private set; }
    public Guid ApprenticeshipKey { get; private set; }
    public short AcademicYear { get; private set; }
    [Precision(15, 5)]
    public decimal Amount { get; private set; }
    public byte CollectionMonth { get; private set; }
    public short CollectionYear { get; private set; }
    public byte DeliveryPeriod { get; private set; }
    public string FundingLineType { get; private set; }
    public Guid EarningsProfileId { get; private set; }
    public string? InstalmentType { get; private set; }
}