using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

public class OrchestrationData : IOrchestrationData
{
    public DurableOrchestrationStatus Status { get; set; }
    public object Entity { get; set; }
}