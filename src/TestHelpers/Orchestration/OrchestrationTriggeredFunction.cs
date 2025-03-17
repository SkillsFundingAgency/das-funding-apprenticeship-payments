using System.Reflection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class OrchestrationTriggeredFunction
{
    public string FunctionName { get; set; }
    public Type ClassType { get; set; }
    public MethodInfo Method { get; set; }
    public TriggerType TriggerType { get; set; }
    public bool HasTaskWithResult { get; set; }
}

public enum TriggerType
{
    Activity,
    Orchestration
}