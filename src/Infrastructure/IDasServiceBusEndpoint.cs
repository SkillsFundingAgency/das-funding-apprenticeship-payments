namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
public interface IDasServiceBusEndpoint
{
    public Task Publish(object @event);
}