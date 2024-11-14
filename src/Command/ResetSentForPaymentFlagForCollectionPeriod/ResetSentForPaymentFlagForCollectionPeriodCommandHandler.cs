using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ResetSentForPaymentFlagForCollectionPeriod;

public class ResetSentForPaymentFlagForCollectionPeriodCommandHandler : IResetSentForPaymentFlagForCollectionPeriodCommandHandler
{
    private readonly ILogger<ResetSentForPaymentFlagForCollectionPeriodCommandHandler> _logger;

    public ResetSentForPaymentFlagForCollectionPeriodCommandHandler(
        ILogger<ResetSentForPaymentFlagForCollectionPeriodCommandHandler> logger)
    {
        _logger = logger;
    }

    public void Process(ResetSentForPaymentFlagForCollectionPeriodCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Model);
        var apprenticeshipKey = command.Model.ApprenticeshipKey;
        
        var paymentsToReset = GetAllPaymentsForRequestedCollectionPeriod(command);

        if (paymentsToReset.Any())
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  Resetting SentForPayment flag on {numberOfPayments} payments for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, paymentsToReset.Length, command.CollectionPeriod, command.CollectionYear);
        }
        else
        {
            _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  No payments to reset SentForPayment flag for collection period {collectionPeriod} & year {collectionYear}", apprenticeshipKey, command.CollectionPeriod, command.CollectionYear);
        }

        foreach (var payment in paymentsToReset)
        {
            payment.SentForPayment = false;
        }
    }

    private PaymentEntityModel[] GetAllPaymentsForRequestedCollectionPeriod(ResetSentForPaymentFlagForCollectionPeriodCommand command)
    {
        return command.Model.Payments
                .Where(x => x.CollectionPeriod == command.CollectionPeriod && x.CollectionYear == command.CollectionYear)
                .ToArray();
    }
}