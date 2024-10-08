namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class SystemTimeStepDefinitions
{
    [Given(@"the date is now (.*)")]
    public static void SetCurrentDate(string dateTime)
    {
        TestSystemClock.SetDateTime(DateTime.Parse(dateTime));
    }
}
