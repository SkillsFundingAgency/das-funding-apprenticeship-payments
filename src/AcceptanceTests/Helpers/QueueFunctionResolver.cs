using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Extensions;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

internal static class QueueFunctionResolver
{
    internal static IEnumerable<QueueTriggeredFunction> GetQueueTriggeredFunctions()
    {
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().FullName.Contains("SFA.DAS"));

        var matchingClasses = allAssemblies.GetClassesWithMethodParameterAttribute<ServiceBusTriggerAttribute>();


        var queueTriggeredFunctions = new List<QueueTriggeredFunction>();
        foreach (var matchingClass in matchingClasses)
        {
            var endpoints = matchingClass.GetMethods()
                .SelectMany(method => method.GetParameters()
                .Where(parameter => parameter.GetCustomAttributes(typeof(ServiceBusTriggerAttribute), false)
                .Any())
                .Select(parameter => new QueueTriggerEndpoint
                {
                    EventType = parameter.ParameterType,
                    MethodInfo = method
                }));

            queueTriggeredFunctions.Add(new QueueTriggeredFunction
            {
                ClassType = matchingClass,
                Endpoints = endpoints
            });
        }
        return queueTriggeredFunctions;
    }
}
