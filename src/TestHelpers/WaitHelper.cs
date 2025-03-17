using NUnit.Framework;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;


public class WaitHelper
{
    private static WaitConfiguration Config => new();

    public static async Task WaitForIt(Func<bool> lookForIt, Func<string> failText)
    {
        var endTime = DateTime.Now.Add(Config.TimeToWait);

        while (DateTime.Now <= endTime)
        {
            if (lookForIt()) return;

            await Task.Delay(Config.TimeToPause);
        }

        Assert.Fail($"{failText()}  Time: {DateTime.Now:G}.");
    }

    public static async Task WaitForItAsync(Func<Task<bool>> lookForIt, string failText)
    {
        var endTime = DateTime.Now.Add(Config.TimeToWait);

        while (DateTime.Now <= endTime)
        {
            if (await lookForIt()) return;

            await Task.Delay(Config.TimeToPause);
        }

        Assert.Fail($"{failText}  Time: {DateTime.Now:G}.");
    }

    public static async Task WaitForIt(Func<bool> lookForIt, string failText)
    {
        var endTime = DateTime.Now.Add(Config.TimeToWait);

        while (DateTime.Now <= endTime)
        {
            if (lookForIt()) return;

            await Task.Delay(Config.TimeToPause);
        }

        Assert.Fail($"{failText}  Time: {DateTime.Now:G}.");
    }

    public static async Task WaitForUnexpected(Func<bool> findUnexpected, string failText)
    {
        var endTime = DateTime.Now.Add(Config.TimeToWait);
        while (DateTime.Now < endTime)
        {
            if (findUnexpected())
            {
                Assert.Fail($"{failText} Time: {DateTime.Now:G}.");
            }

            await Task.Delay(Config.TimeToPause);
        }
    }
}
public class WaitConfiguration
{
    public TimeSpan TimeToWait { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan TimeToPause { get; set; } = TimeSpan.FromMilliseconds(50);
}