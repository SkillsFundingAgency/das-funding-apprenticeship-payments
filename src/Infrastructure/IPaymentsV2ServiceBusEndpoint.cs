using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public interface IPaymentsV2ServiceBusEndpoint
{
    Task Send(object message);
    Task Publish(CalculatedRequiredLevyAmount @event);
}