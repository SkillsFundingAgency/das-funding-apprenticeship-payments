using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class FunctionInvoker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<OrchestrationTriggeredFunction> _orchestrationFunctions;
    private readonly ConcurrentDictionary<string, ManualResetEventSlim> _runningTasks = new();

    public FunctionInvoker(IServiceProvider serviceProvider, IEnumerable<OrchestrationTriggeredFunction> orchestrationTriggeredFunctions)
    {
        _serviceProvider = serviceProvider;
        _orchestrationFunctions = orchestrationTriggeredFunctions;
    }

    public object? Invoke(string functionName, object?[]? parameters)
    {
        var triggeredFunction = _orchestrationFunctions.First(x => x.FunctionName == functionName);
        var handler = _serviceProvider.GetService(triggeredFunction.ClassType);
        return triggeredFunction.Method.Invoke(handler, parameters);
    }

    public async Task<TResult> InnerInvokeAsync<TResult>(string functionName, object?[]? parameters, IServiceProvider serviceProvider)
    {
        var triggeredFunction = _orchestrationFunctions.First(x => x.FunctionName == functionName);
        var handler = serviceProvider.GetService(triggeredFunction.ClassType);
        var invoked = triggeredFunction.Method.Invoke(handler, parameters);

        if(invoked is Task<TResult> task)
        {
            return await task;
        }

        if(invoked is TResult result)
        {
            return result;
        }

        if (invoked is Task taskResult)
        {
            await taskResult;
            return default;
        }

        return default;
    }

    public async Task<TResult> InvokeAsync<TResult>(string functionName, object?[]? parameters)
    {
        var scope = _serviceProvider.CreateScope();
        
        try
        {
            var scopedProvider = scope.ServiceProvider;
            return await InnerInvokeAsync<TResult>(functionName, parameters, scopedProvider);
        }
        finally
        {
            await Task.Delay(500);
            scope.Dispose();
        }
    }
}
