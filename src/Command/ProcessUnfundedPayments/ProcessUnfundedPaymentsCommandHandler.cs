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
        var paymentsToSend = command.Model.Payments.Where(x => x.CollectionPeriod == command.CollectionMonth && x.SentForPayment == false);

        _logger.LogInformation($"Apprenticeship Key: {command.Model.ApprenticeshipKey} -  Publishing { paymentsToSend.Count() } payments for collection period { command.CollectionMonth }");

        foreach (var payment in paymentsToSend)
        {
            await _messageSession.Publish(_eventBuilder.Build(payment, command.Model.ApprenticeshipKey));
            payment.SentForPayment = true;
        }
    }
}