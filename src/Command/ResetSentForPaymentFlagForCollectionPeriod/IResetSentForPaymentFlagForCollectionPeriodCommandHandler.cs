﻿namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ResetSentForPaymentFlagForCollectionPeriod;

public interface IResetSentForPaymentFlagForCollectionPeriodCommandHandler
{
    void Process(ResetSentForPaymentFlagForCollectionPeriodCommand command);
}