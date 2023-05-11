﻿namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class FinalisedOnProgammeLearningPaymentEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public byte CollectionMonth { get; set; }
    public short CollectionYear { get; set; }
    public Decimal Amount { get; set; }
}