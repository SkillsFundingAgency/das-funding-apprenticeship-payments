namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
public interface IDasServiceBusEndpoint
{
    public Task Send(object message);
    public Task Publish(object @event);
}