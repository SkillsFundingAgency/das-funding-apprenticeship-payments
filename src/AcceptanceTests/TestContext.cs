namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

public class TestContext : IDisposable
{
    public TestFunction? TestFunction { get; set; }
    public SqlDatabase? SqlDatabase { get; set; }
    public TestEndpointInstanceHandler EndpointInstanceHandler { get; set; }

    public AutoFixture.Fixture Fixture = new();

    public List<long> Ulns = new();

    public TestContext()
    {
        EndpointInstanceHandler = new TestEndpointInstanceHandler();
    }

    public List<T> ReceivedEvents<T>()
    {
        return EndpointInstanceHandler.ReceivedEvents<T>();
    }

    public void Dispose()
    {
        TestFunction?.Dispose();
        SqlDatabase?.Dispose();
    }
}