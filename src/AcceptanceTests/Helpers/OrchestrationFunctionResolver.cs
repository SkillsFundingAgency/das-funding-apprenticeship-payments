using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;
using System.Reflection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

internal static class OrchestrationFunctionResolver
{
    internal static IEnumerable<OrchestrationTriggeredFunction> GetOrchestrationTriggeredFunctions()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().FullName.Contains("SFA.DAS"));

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
                Method = method
            });
        }

        matchingClasses = allAssemblies.GetClassesWithMethodParameterAttribute<ActivityTriggerAttribute>();

        foreach (var matchingClass in matchingClasses)
        {
            var method = matchingClass.GetMethodWithParameterAttribute<ActivityTriggerAttribute>();

            var functionAttribute = method.GetCustomAttribute<FunctionAttribute>();

            orchestrationTriggeredFunctions.Add(new OrchestrationTriggeredFunction
            {
                FunctionName = functionAttribute.Name,
                ClassType = matchingClass,
                Method = method
            });
        }

        return orchestrationTriggeredFunctions;
    }
}