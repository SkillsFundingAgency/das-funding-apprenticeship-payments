namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

public interface ISystemClockService
{
    /// <summary>Retrieves the current system time in UTC.</summary>
    DateTimeOffset UtcNow { get; }
    DateTime Now { get; }
}