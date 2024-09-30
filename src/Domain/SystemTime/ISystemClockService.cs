namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

public interface ISystemClockService
{
    /// <summary>Retrieves the current system time in UTC.</summary>
    DateTimeOffset UtcNow { get; }
    DateTime Now { get; }
}