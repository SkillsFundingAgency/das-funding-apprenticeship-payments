using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public interface IPaymentsV2ServiceBusEndpoint
{
    Task Send(object message);
    Task Publish(CalculateOnProgrammePayment @event);
}