namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public static class QueueNames
{
    public const string EarningsGenerated = "das-funding-payments-earningsgenerated";
    public const string EarningsRecalculated = "das-funding-payments-earningsrecalculated";
    public const string ReleasePayments = "das-funding-payments-releasepayments";
    public const string FinalisedOnProgammeLearningPayment = "das-funding-payments-finalisedpaymentgenerated";
    public const string CalculatedRequiredLevyAmount = "sfa-das-payments-fundingsource-levy-transaction";
    public const string PaymentsFrozen = "das-funding-payments-paymentsfrozen";
    public const string PaymentsUnfrozen = "das-funding-payments-paymentsunfrozen";
    public const string ResetSentForPaymentFlag = "das-funding-payments-resetsentforpaymentflag";
}