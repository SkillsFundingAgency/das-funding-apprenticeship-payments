using System.Collections.Generic;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;

public abstract class InputBase
{
    public string OrchestrationInstanceId { get; set; }

    public InputBase(string instanceId)
    {
        OrchestrationInstanceId = instanceId;
    }
}

public static class InputBaseExtensions
{
    public static Dictionary<string, object> GetLoggingScope(this InputBase input)
    {
        return new Dictionary<string, object>
        {
            ["OrchestrationInstanceId"] = input.OrchestrationInstanceId
        };
    }
}