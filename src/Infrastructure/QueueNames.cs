namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public static class QueueNames
{
    public const string EarningsGenerated = "das-funding-payments-earnings-generated";
    public const string ReleasePayments = "das-funding-payments-release-payments";
    public const string FinalisedOnProgammeLearningPayment = "sfa.das.funding.payments.finalisedpaymentgenerated";
    public const string CalculatedRequiredLevyAmount = "sfa-das-payments-fundingsource-levy";
}