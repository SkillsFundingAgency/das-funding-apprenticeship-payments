using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class EmployerDetails : IEvent
{
    public long EmployingAccountId { get; set; }
    public long FundingCommitmentId { get; set; }
    public long FundingAccountId { get; set; }
}