namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public interface IPaymentsV2ServiceBusEndpoint
{
    public Task Send(object message);
}