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
        var apprenticeshipKey = command.Model.ApprenticeshipKey;

        if(command.Model.PaymentsFrozen)
        {
            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} - Payments are frozen, no payments will be published", apprenticeshipKey);
            return;
        }

        var paymentsToSend = command.Model.Payments
            .Where(x => x.CollectionPeriod == command.CollectionPeriod && x.CollectionYear == command.CollectionYear && !x.SentForPayment)
            .ToArray();

        if (paymentsToSend.Any())
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  Publishing {numberOfPayments} payments for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, paymentsToSend.Length, command.CollectionPeriod, command.CollectionYear);
        }
        else
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  No payments to publish for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, command.CollectionPeriod, command.CollectionYear);
        }


        foreach (var payment in paymentsToSend)
        {
            var finalisedOnProgammeLearningPaymentEvent = _eventBuilder.Build(payment, command.Model);
            using (var monitor = _logger.LogPerformance("busEndpoint Publish finalisedOnProgammeLearningPaymentEvent"))
            {
                await _busEndpoint.Publish(finalisedOnProgammeLearningPaymentEvent);
            }


            _logger.LogInformation("ApprenticeshipKey: {0} Publishing FinalisedOnProgammeLearningPaymentEvent: {1}",
                finalisedOnProgammeLearningPaymentEvent.ApprenticeshipKey,
                finalisedOnProgammeLearningPaymentEvent.SerialiseForLogging());

            payment.SentForPayment = true;
        }
    }
}