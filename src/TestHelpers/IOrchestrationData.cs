using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

public interface IOrchestrationData
{
    DurableOrchestrationStatus Status { get; set; }
    object Entity { get; set; }
}