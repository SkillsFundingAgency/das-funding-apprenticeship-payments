﻿namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class ReleasePaymentsCommand
{
    public byte CollectionPeriod { get; set; }
    public short CollectionYear { get; set; }
}