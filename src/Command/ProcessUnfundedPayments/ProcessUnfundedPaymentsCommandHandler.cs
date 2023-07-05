using Microsoft.Extensions.Logging;
using NServiceBus;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

public class ProcessUnfundedPaymentsCommandHandler : IProcessUnfundedPaymentsCommandHandler
{
    private readonly IMessageSession _messageSession;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;
    private readonly ILogger<ProcessUnfundedPaymentsCommandHandler> _logger;

    public ProcessUnfundedPaymentsCommandHandler(IMessageSession messageSession, IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder, ILogger<ProcessUnfundedPaymentsCommandHandler> logger)
    {
        _messageSession = messageSession;
        _eventBuilder = eventBuilder;
        _logger = logger;
    }

    public async Task Process(ProcessUnfundedPaymentsCommand command)
    {
        var paymentsToSend = command.Model.Payments
            .Where(x => x.CollectionPeriod == command.CollectionPeriod && x.SentForPayment == false)
            .ToArray();

        _logger.LogInformation(paymentsToSend.Any()
            ? $"Apprenticeship Key: {command.Model.ApprenticeshipKey} -  Publishing {paymentsToSend.Count()} payments for collection period {command.CollectionPeriod}"
            : $"Apprenticeship Key: {command.Model.ApprenticeshipKey} -  No payments to publish for collection period {command.CollectionPeriod}");

        foreach (var payment in paymentsToSend)
        {
            await _messageSession.Publish(_eventBuilder.Build(payment, command.Model.ApprenticeshipKey, (short)command.Model.Payments.Count));
            payment.SentForPayment = true;
        }
    }
}