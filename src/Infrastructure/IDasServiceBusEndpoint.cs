namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
public interface IDasServiceBusEndpoint
{
    Task Publish(object @event);
}