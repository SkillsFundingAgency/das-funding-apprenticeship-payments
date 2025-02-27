using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;
using System.Reflection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

internal static class OrchestrationFunctionResolver
{
    internal static IEnumerable<OrchestrationTriggeredFunction> GetTriggeredFunctions()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().FullName.Contains("SFA.DAS"));

        var orchestrationTriggeredFunctions = GetOrchestrationTriggeredFunctions(allAssemblies);

        orchestrationTriggeredFunctions.AddRange(GetActivityTriggeredFunctions(allAssemblies));

        return orchestrationTriggeredFunctions;
    }

    private static List<OrchestrationTriggeredFunction> GetOrchestrationTriggeredFunctions(IEnumerable<Assembly> allAssemblies)
    {
        var matchingClasses = allAssemblies.GetClassesWithMethodParameterAttribute<OrchestrationTriggerAttribute>();

        var orchestrationTriggeredFunctions = new List<OrchestrationTriggeredFunction>();

        foreach (var matchingClass in matchingClasses)
        {
            var method = matchingClass.GetMethodWithParameterAttribute<OrchestrationTriggerAttribute>();

            var functionAttribute = method.GetCustomAttribute<FunctionAttribute>();

            orchestrationTriggeredFunctions.Add(new OrchestrationTriggeredFunction
            {
                FunctionName = functionAttribute.Name,
                ClassType = matchingClass,
                Method = method,
                TriggerType = TriggerType.Orchestration,
                HasTaskWithResult = IsTaskWithResult(method)
            });
        }

        return orchestrationTriggeredFunctions;
    }

    private static List<OrchestrationTriggeredFunction> GetActivityTriggeredFunctions(IEnumerable<Assembly> allAssemblies)
    {
        var orchestrationTriggeredFunctions = new List<OrchestrationTriggeredFunction>();

        var matchingClasses = allAssemblies.GetClassesWithMethodParameterAttribute<ActivityTriggerAttribute>();

        foreach (var matchingClass in matchingClasses)
        {
            var method = matchingClass.GetMethodWithParameterAttribute<ActivityTriggerAttribute>();

            var functionAttribute = method.GetCustomAttribute<FunctionAttribute>();

            orchestrationTriggeredFunctions.Add(new OrchestrationTriggeredFunction
            {
                FunctionName = functionAttribute.Name,
                ClassType = matchingClass,
                Method = method,
                TriggerType = TriggerType.Activity,
                HasTaskWithResult = IsTaskWithResult(method)
            });
        }

        return orchestrationTriggeredFunctions;
    }

    private static bool IsTaskWithResult(MethodInfo methodInfo)
    {
        Type returnType = methodInfo.ReturnType;

        // Check if return type is a Task
        if (returnType == typeof(Task))
            return false;

        // Check if return type is a generic Task<>
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            return true;

        return false;
    }
}