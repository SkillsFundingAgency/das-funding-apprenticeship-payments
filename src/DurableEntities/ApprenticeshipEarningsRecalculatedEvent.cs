using System.Collections.Generic;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

//todo replace this with a reference to the event from the earnings domain once FLP-315 is complete
public class ApprenticeshipEarningsRecalculatedEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public List<EarningsRecalculatedDeliveryPeriod> DeliveryPeriods { get; set; }
}

public class EarningsRecalculatedDeliveryPeriod
{
    public byte Period { get; set; }
    public byte CalendarMonth { get; set; }
    public short CalenderYear { get; set; }
    public short AcademicYear { get; set; }
    public Decimal LearningAmount { get; set; }
    public string FundingLineType { get; set; }
}