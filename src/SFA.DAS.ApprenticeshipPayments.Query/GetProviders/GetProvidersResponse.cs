namespace SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders;

public class Provider
{
    public long Ukprn { get; }

    public Provider(long ukprn)
    {
        Ukprn = ukprn;
    }
}

public class GetProvidersResponse
{
    public GetProvidersResponse(IEnumerable<Provider> providers)
    {
        Providers = providers;
    }

    public IEnumerable<Provider> Providers { get; }
}