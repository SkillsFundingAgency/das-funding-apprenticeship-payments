using Microsoft.AspNetCore.TestHost;
using System.Reflection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

internal class QueueTriggeredFunction
{
    public Type ClassType { get; set; }
    public IEnumerable<QueueTriggerEndpoint> Endpoints { get; set; }
}

internal class QueueTriggerEndpoint
{
    public Type EventType { get; set; }
    public MethodInfo MethodInfo { get; set; }
}

internal static class QueueTriggeredFunctionExtensions
{
    internal static async Task PublishEvent<T>(
        this IEnumerable<QueueTriggeredFunction> queueTriggeredFunctions,
        IServiceProvider serviceProvider, 
        T eventObject)
    {
        var function = queueTriggeredFunctions.GetFunctionForEvent<T>();

        var handler = serviceProvider.GetService(function.ClassType);
        if (handler == null)
        {
            throw new Exception($"No azure function was found to handle event {typeof(T)}");
        }

        var method = function!.Endpoints.First(x => x.EventType == typeof(T)).MethodInfo;
        var parameters = function.GetParameters(method, serviceProvider, eventObject);

        try
        {
            await (Task)method.Invoke(handler, parameters.ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to invoke method {method.Name} on class {function.ClassType.Name} error:{ex.Message}");// Some of the tests verify the behaviour on handler errors, for this reason the exception is swallowed
        }
    }

    private static object[] GetParameters<T>(
        this QueueTriggeredFunction function, 
        MethodInfo method, 
        IServiceProvider serviceProvider,
        T eventObject)
    {
        var parameters = new List<object>();

        foreach (var parameter in method.GetParameters())
        {
            if (parameter.ParameterType == typeof(T))
            {
                parameters.Add(eventObject!);
            }
            else
            {
                var service = serviceProvider.GetService(parameter.ParameterType);
                if (service == null)
                {
                    throw new Exception($"While trying to resolve parameters for queuetriggered azure function, no service was found for type {parameter.ParameterType}");
                }
                parameters.Add(service);
            }
        }

        return parameters.ToArray();
    }

    internal static QueueTriggeredFunction GetFunctionForEvent<T>(this IEnumerable<QueueTriggeredFunction> queueTriggeredFunctions)
    {
        var function = queueTriggeredFunctions.FirstOrDefault(x => x.Endpoints.Where(e => e.EventType == typeof(T)).Any());

        if (function == null)
        {
            throw new Exception($"No QueueTriggeredFunction registered for event {typeof(T)}");
        }

        return function;
    }

}
