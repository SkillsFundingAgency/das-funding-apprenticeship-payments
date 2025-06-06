﻿using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class InMemoryDurableTaskClient(string name, FunctionInvoker functionInvoker) : DurableTaskClient(name)
{
    private readonly Dictionary<string, InMemoryTaskOrchestrationContext> _orchestrations = new();

    public override async Task<string> ScheduleNewOrchestrationInstanceAsync(TaskName orchestratorName, object? input = null, StartOrchestrationOptions? options = null, CancellationToken cancellation = default)
    {
        var instanceId = Guid.NewGuid().ToString();
        
        var context = new InMemoryTaskOrchestrationContext(orchestratorName, instanceId, functionInvoker);
        _orchestrations[instanceId] = context;

        if(input != null)
            context.AddInput(input);

        var parameters = new List<object> { context };

        await context.TriggerFunction(orchestratorName); 

        return await Task.FromResult(instanceId);
    }

    public override async Task<OrchestrationMetadata> WaitForInstanceCompletionAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
    {
        var orchestration = _orchestrations[instanceId];

        var endTime = DateTime.Now.Add(TimeSpan.FromSeconds(60));
        while (DateTime.Now < endTime)
        {
            if (orchestration.IsCompleted)
            {
                return new OrchestrationMetadata(instanceId, orchestration.Name);
            }

            await Task.Delay(100, cancellation);
        }

        return new OrchestrationMetadata(instanceId, orchestration.Name);
    }

    public override AsyncPageable<OrchestrationMetadata> GetAllInstancesAsync(OrchestrationQuery? filter = null)
    {
        var orchestrations = _orchestrations.Select(x => new OrchestrationMetadata(x.Value.Name, x.Key));

        return new CombinedPageable<OrchestrationMetadata>(orchestrations);
    }

    public override Task<Microsoft.DurableTask.Client.PurgeResult> PurgeInstanceAsync(string instanceId, CancellationToken cancellation = default)
    {
        _orchestrations.Remove(instanceId);
        return Task.FromResult(new Microsoft.DurableTask.Client.PurgeResult(1));
    }

    #region NotImplemented
    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public override Task<OrchestrationMetadata?> GetInstancesAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Microsoft.DurableTask.Client.PurgeResult> PurgeAllInstancesAsync(PurgeInstancesFilter filter, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task RaiseEventAsync(string instanceId, string eventName, object? eventPayload = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task ResumeInstanceAsync(string instanceId, string? reason = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task SuspendInstanceAsync(string instanceId, string? reason = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task TerminateInstanceAsync(string instanceId, object? output = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<OrchestrationMetadata> WaitForInstanceStartAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
    #endregion NotImplemented
}