using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class InMemoryTaskOrchestrationContext : TaskOrchestrationContext
{
    private List<object> _inputs;
    private TaskName _taskName;
    private string _instanceId;
    private object _customStatus;
    private FunctionInvoker _functionInvoker;
    private bool _isFunctionCallCompleted = false;
    private readonly Dictionary<string, InMemoryTaskOrchestrationContext> _orchestrations = new();
    private readonly Dictionary<string, bool> _runningActivitiesCompleted = new();// The bool represents if the activity is completed

    public override TaskName Name => _taskName;
    public override string InstanceId => _instanceId;
    public override bool IsReplaying => false;
    public bool IsCompleted => _isFunctionCallCompleted && _orchestrations.All(x=>x.Value.IsCompleted) && _runningActivitiesCompleted.All(x=>x.Value);

    public InMemoryTaskOrchestrationContext(
        TaskName taskName, 
        string instanceId, 
        FunctionInvoker functionInvoker)
    {
        _taskName = taskName;
        _instanceId = instanceId;
        _inputs = new List<object>();
        _functionInvoker = functionInvoker;
    }

    public async Task TriggerFunction(string functionName)
    {
        try
        {
            await _functionInvoker.InvokeAsync<Task>(_instanceId, functionName, new object[] { this });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured calling TriggerFunction for '{functionName}' error:{e.Message}");
        }
        finally
        {
            _isFunctionCallCompleted = true;
        }
    }

    public void AddInput(object input)
    {
        _inputs.Add(input);
    }

    public override T? GetInput<T>() where T : default
    {
        return _inputs.OfType<T>().FirstOrDefault();
    }

    public override void SetCustomStatus(object? customStatus)
    {
        _customStatus = customStatus;
    }

    public override ParentOrchestrationInstance? Parent => throw new NotImplementedException();

    public override DateTime CurrentUtcDateTime => throw new NotImplementedException();

    protected override ILoggerFactory LoggerFactory => throw new NotImplementedException();

    public override async Task<TResult> CallActivityAsync<TResult>(TaskName name, object? input = null, TaskOptions? options = null)
    {
        var activityId = $"{name}{Guid.NewGuid()}";
        _runningActivitiesCompleted[activityId] = false;

        try
        {
            return await _functionInvoker.InvokeAsync<TResult>(_instanceId, name, new object[] { input });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured calling CallActivityAsync for '{name}' error:{e.Message}");
        }
        finally
        {
            _runningActivitiesCompleted[activityId] = true;
        }

        return default;
    }

    public override async Task<TResult> CallSubOrchestratorAsync<TResult>(TaskName orchestratorName, object? input = null, TaskOptions? options = null)
    {
        var context = new InMemoryTaskOrchestrationContext(orchestratorName, Guid.NewGuid().ToString(), _functionInvoker);
        context.AddInput(input);
        _orchestrations[context.InstanceId] = context;
        try
        {
            context.TriggerFunction(orchestratorName);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured calling CallSubOrchestratorAsync for '{orchestratorName}' error:{e.Message}");
        }
        return default;
    }

    public override void ContinueAsNew(object? newInput = null, bool preserveUnprocessedEvents = true)
    {
        throw new NotImplementedException();
    }

    public override Task CreateTimer(DateTime fireAt, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Guid NewGuid()
    {
        throw new NotImplementedException();
    }

    public override void SendEvent(string instanceId, string eventName, object payload)
    {
        throw new NotImplementedException();
    }

    public override Task<T> WaitForExternalEvent<T>(string eventName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
