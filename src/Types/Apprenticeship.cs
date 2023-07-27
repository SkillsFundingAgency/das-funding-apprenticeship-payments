using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class Apprenticeship : IEvent
{
    public DateTime StartDate { get; set; }
    public long ApprovalsApprenticeshipId { get; set; }
}