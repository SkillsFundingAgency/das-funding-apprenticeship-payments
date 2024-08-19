namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;

public class SystemClockService : ISystemClockService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateTime Now => DateTime.Now;
}