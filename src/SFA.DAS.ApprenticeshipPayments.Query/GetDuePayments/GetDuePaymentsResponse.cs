namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;

public class Payment
{
    public Payment(Guid key)
    {
        Key = key;
    }

    public Guid Key { get; }
}

public class GetDuePaymentsResponse
{
    public GetDuePaymentsResponse(IEnumerable<Payment> payments)
    {
        Payments = payments;
    }

    public IEnumerable<Payment> Payments { get; }
}