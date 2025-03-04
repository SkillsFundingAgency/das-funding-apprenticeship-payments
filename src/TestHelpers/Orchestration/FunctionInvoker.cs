using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class FunctionInvoker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<OrchestrationTriggeredFunction> _orchestrationFunctions;

    public FunctionInvoker(IServiceProvider serviceProvider, IEnumerable<OrchestrationTriggeredFunction> orchestrationTriggeredFunctions)
    {
        _serviceProvider = serviceProvider;
        _orchestrationFunctions = orchestrationTriggeredFunctions;
    }

#pragma warning disable CS8603 // return type can be null
    public async Task<TResult> InvokeAsync<TResult>(string instanceId, string functionName, object?[]? parameters)
    {
        var triggeredFunction = _orchestrationFunctions.First(x => x.FunctionName == functionName);

        var handler = _serviceProvider.GetService(triggeredFunction.ClassType);

        if(handler == null)
            throw new InvalidOperationException($"Could not find handler for {triggeredFunction.ClassType}");

        switch (triggeredFunction.TriggerType)
        {
            case TriggerType.Orchestration:
                await (Task)triggeredFunction.Method.Invoke(handler, parameters)!;
                return default;

            case TriggerType.Activity:
                var response = await InvokeActivityAsync<TResult>(triggeredFunction, handler, parameters);
                return response;
        }

        throw new ArgumentOutOfRangeException("Trigger type not valid");
    }
#pragma warning restore CS8603

    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private async Task<TResult> InvokeActivityAsync<TResult>(OrchestrationTriggeredFunction triggeredFunction, object handler, object?[]? parameters)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (triggeredFunction.HasTaskWithResult)
            {
                return await (Task<TResult>)triggeredFunction.Method.Invoke(handler, parameters)!;
            }

            await (Task)triggeredFunction.Method.Invoke(handler, parameters)!;
            return default!;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}