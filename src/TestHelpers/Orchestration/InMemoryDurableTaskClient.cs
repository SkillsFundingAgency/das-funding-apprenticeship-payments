using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class InMemoryDurableTaskClient : DurableTaskClient
{
    private readonly Dictionary<string, InMemoryTaskOrchestrationContext> _orchestrations = new();
    private FunctionInvoker _functionInvoker;

    public InMemoryDurableTaskClient(
        string name,
        FunctionInvoker functionInvoker) : base(name)
    {
        _functionInvoker = functionInvoker;
    }

    public override async Task<string> ScheduleNewOrchestrationInstanceAsync(TaskName orchestratorName, object? input = null, StartOrchestrationOptions? options = null, CancellationToken cancellation = default)
    {
        var instanceId = Guid.NewGuid().ToString();
        
        var context = new InMemoryTaskOrchestrationContext(orchestratorName, instanceId, _functionInvoker);
        _orchestrations[instanceId] = context;

        context.AddInput(input);
        var parameters = new List<object> { context };
        context.TriggerFunction(orchestratorName);
        return instanceId;
    }

    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public override AsyncPageable<OrchestrationMetadata> GetAllInstancesAsync(OrchestrationQuery? filter = null)
    {
        var orchestrations = _orchestrations.Select(x => new OrchestrationMetadata(x.Value.Name, x.Key));

        return new CombinedPageable<OrchestrationMetadata>(orchestrations);
    }

    public override Task<OrchestrationMetadata?> GetInstancesAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Microsoft.DurableTask.Client.PurgeResult> PurgeAllInstancesAsync(PurgeInstancesFilter filter, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Microsoft.DurableTask.Client.PurgeResult> PurgeInstanceAsync(string instanceId, CancellationToken cancellation = default)
    {
        _orchestrations.Remove(instanceId);
        return Task.FromResult(new Microsoft.DurableTask.Client.PurgeResult(1));
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

            await Task.Delay(1000);
        }

        return new OrchestrationMetadata(instanceId, orchestration.Name);
    }

    public override Task<OrchestrationMetadata> WaitForInstanceStartAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}