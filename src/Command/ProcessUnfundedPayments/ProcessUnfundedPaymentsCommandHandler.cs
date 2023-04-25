using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

public class ProcessUnfundedPaymentsCommandHandler : IProcessUnfundedPaymentsCommandHandler
{
    private readonly IMessageSession _messageSession;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;

    public ProcessUnfundedPaymentsCommandHandler(IMessageSession messageSession, IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder)
    {
        _messageSession = messageSession;
        _eventBuilder = eventBuilder;
    }

    public async Task Process(ProcessUnfundedPaymentsCommand command)
    {
        var paymentsToSend = command.Model.Payments.Where(x => x.PaymentPeriod == command.CollectionMonth && x.SentForPayment == false);
        foreach (var payment in paymentsToSend)
        {
            await _messageSession.Publish(_eventBuilder.Build(payment, command.Model.ApprenticeshipKey));
            payment.SentForPayment = true;
        }
    }
}