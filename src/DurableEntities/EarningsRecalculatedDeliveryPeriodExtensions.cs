using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

public static class EarningsRecalculatedDeliveryPeriodExtensions
{
    public static List<Earning> ToEarnings(this List<DeliveryPeriod> earningsRecalculatedDeliveryPeriods, Guid earningsProfileId)
    {
        return earningsRecalculatedDeliveryPeriods.Select(dp => new Earning(dp.AcademicYear, dp.Period,
            dp.LearningAmount, dp.CalenderYear, dp.CalendarMonth, dp.FundingLineType, earningsProfileId)).ToList();
    }
}