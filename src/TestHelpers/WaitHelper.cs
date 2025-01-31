﻿using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

public static class WaitConfigurationHelper
{
    public static WaitConfiguration WaitConfiguration
    {
        get
        {
            if (_waitConfiguration == null)
            {
                _waitConfiguration = new WaitConfiguration();
                new ConfigurationBuilder()
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .Build()
                    .GetSection("WaitConfiguration")
                    .Bind(_waitConfiguration);
            }

            return _waitConfiguration;
        }
    }

    private static WaitConfiguration? _waitConfiguration;
}

public class WaitHelper
{
    private static WaitConfiguration Config => WaitConfigurationHelper.WaitConfiguration;

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