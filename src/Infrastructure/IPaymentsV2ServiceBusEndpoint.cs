using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public interface IPaymentsV2ServiceBusEndpoint
{
    Task Publish(CalculatedRequiredLevyAmount @event);
}