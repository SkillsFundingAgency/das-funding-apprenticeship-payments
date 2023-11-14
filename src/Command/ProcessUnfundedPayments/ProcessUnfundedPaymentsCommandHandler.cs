namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

public class ProcessUnfundedPaymentsCommandHandler : IProcessUnfundedPaymentsCommandHandler
{
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;
    private readonly ILogger<ProcessUnfundedPaymentsCommandHandler> _logger;

    public ProcessUnfundedPaymentsCommandHandler(IDasServiceBusEndpoint busEndpoint, IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder, ILogger<ProcessUnfundedPaymentsCommandHandler> logger)
    {
        _busEndpoint = busEndpoint;
        _eventBuilder = eventBuilder;
        _logger = logger;
    }

    public async Task Process(ProcessUnfundedPaymentsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Model);

        var paymentsToSend = command.Model.Payments
            .Where(x => x.CollectionPeriod == command.CollectionPeriod && x.CollectionYear == command.CollectionYear && !x.SentForPayment)
            .ToArray();

        _logger.LogInformation(paymentsToSend.Any()
            ? $"Apprenticeship Key: {command.Model.ApprenticeshipKey} -  Publishing {paymentsToSend.Length} payments for collection period {command.CollectionPeriod} & year {command.CollectionYear}"
            : $"Apprenticeship Key: {command.Model.ApprenticeshipKey} -  No payments to publish for collection period {command.CollectionPeriod} & year {command.CollectionYear}");

        foreach (var payment in paymentsToSend)
        {
            var finalisedOnProgammeLearningPaymentEvent = _eventBuilder.Build(payment, command.Model);
            await _busEndpoint.Publish(finalisedOnProgammeLearningPaymentEvent);

            _logger.LogInformation("ApprenticeshipKey: {0} Publishing FinalisedOnProgammeLearningPaymentEvent: {1}",
                finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey,
                finalisedOnProgammeLearningPaymentEvent.SerialiseForLogging());

            payment.SentForPayment = true;
        }
    }
}