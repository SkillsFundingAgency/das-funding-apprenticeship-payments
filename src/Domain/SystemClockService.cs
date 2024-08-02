namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain;

public interface ISystemClockService
{
    /// <summary>Retrieves the current system time in UTC.</summary>
    DateTimeOffset UtcNow { get; }
    DateTime Now { get; }
}

public class SystemClockService : ISystemClockService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateTime Now => DateTime.Now;
}