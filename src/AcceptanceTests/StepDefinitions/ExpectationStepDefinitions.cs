using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class ExpectationStepDefinitions
{
    private static List<ExpectedPayments> _expectedPayments = new List<ExpectedPayments>();
    private readonly ScenarioContext _scenarioContext;

    public ExpectationStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    public static void AddExpectedPayment(ExpectedPayments expectedPayments)
    {
        _expectedPayments.Add(expectedPayments);
    }

    [BeforeScenario]
    public void BeforeEachScenario()
    {
        _expectedPayments = new List<ExpectedPayments>();
    }

    [AfterStep]
    public async Task AfterEachStep()
    {
        await Task.Delay(10000);
    }

    [AfterScenario]
    public async Task AfterEachScenario()
    {
        if (!_expectedPayments.Any())
            return;

        var expectedCalculatedPayments = _expectedPayments.Where(x => x.ExpectedType == ExpectedType.Calculated).ToList();
        await ValidateCalculatedPayments(expectedCalculatedPayments);

        var expectedReleasedPayments = _expectedPayments.Where(x => x.ExpectedType == ExpectedType.Released).ToList();
        await ValidateReleasedPayments(expectedReleasedPayments);
    }

    [Then(@"there are (.*) payments of (.*)")]
    public void AddExpectedCalulatedPayments(int paymentCount, decimal paymentAmount)
    {
        _expectedPayments.Add(new ExpectedPayments { 
            ExpectedType = ExpectedType.Calculated, 
            Count = paymentCount, 
            Amount = paymentAmount, 
            ApprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"] 
        });
    }

    [Then(@"for academic year (.*) there are (.*) payments of (.*) released")]
    public void AddExpectedReleasedPayments(string academicYear, int paymentCount, decimal paymentAmount)
    {
        _expectedPayments.Add(new ExpectedPayments { 
            ExpectedType = ExpectedType.Released, 
            AcademicYear = short.Parse(academicYear.Replace("/", "")), 
            Count = paymentCount, 
            Amount = paymentAmount, 
            ApprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"] 
        });
    }

    private static async Task ValidateCalculatedPayments(List<ExpectedPayments> expected)
    {
        if (expected == null || !expected.Any()) return;

        var processedEvents = new List<PaymentsGeneratedEvent>();

        await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Except(processedEvents).Any(e =>
        {
            var outcome = true;

            processedEvents.Add(e);

            foreach (var expectedPayment in _expectedPayments)
            {
                expectedPayment.ActualCountFound = e.Payments.Where(p =>
                    (p.Amount == expectedPayment.Amount || expectedPayment.Amount == null) &&
                    (p.AcademicYear == expectedPayment.AcademicYear || expectedPayment.AcademicYear == null)
                    ).Count(); 

                if (!expectedPayment.MeetsExpectations())
                {
                    outcome = false;
                }
            }

            return outcome;
        }),
        () =>
        {
            if (!processedEvents.Any())
                return "Expected to find PaymentsGeneratedEvent, but none where received";

            return _expectedPayments.FailMessages();
        });
    }

    private static async Task ValidateReleasedPayments(List<ExpectedPayments> expected)
    {
        if (expected == null || !expected.Any()) return;

        var processedEvents = new List<FinalisedOnProgammeLearningPaymentEvent>();

        await WaitHelper.WaitForIt(() => FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Except(processedEvents).Any(e =>
        {
            processedEvents.Add(e);

            var matchingExpectedPayment = expected.FirstOrDefault(x => x.AcademicYear == e.CollectionYear && x.Amount == e.Amount && x.ApprenticeshipKey == e.ApprenticeshipKey);
            if(matchingExpectedPayment != null)
            {
                matchingExpectedPayment.ActualCountFound++;
            }

            return expected.All(x=>x.MeetsExpectations());
        }),
        () =>
        {
            if (!processedEvents.Any())
                return "Expected to find FinalisedOnProgammeLearningPaymentEvent, but none where received";

            return _expectedPayments.FailMessages();
        });

    }
}

public class ExpectedPayments
{
    public Guid ApprenticeshipKey { get; set; }
    public decimal? Amount { get; set; }
    public int Count { get; set; }
    public int ActualCountFound { get; set; }
    public short? AcademicYear { get; set; }
    public ExpectedType ExpectedType { get; set; }
}

public enum ExpectedType
{
    Calculated,
    Released
}

public static class ExpectedPaymentsExtensions
{
    public static bool MeetsExpectations(this ExpectedPayments expected)
    {
        return expected.Count == expected.ActualCountFound;
    }

    public static string FailMessages(this List<ExpectedPayments> expected)
    {
        var message = string.Empty;

        foreach (var expectedPayment in expected.Where(x => !x.MeetsExpectations()))
        {
            message += expectedPayment.FailMessage();
        }

        return message;
    }

    public static string FailMessage(this ExpectedPayments expected)
    {
        var message = $"Expected {expected.Count}";

        if (expected.Amount.HasValue)
            message += $" payments of {expected.Amount}";

        if (expected.AcademicYear.HasValue)
            message += $" for academic year {expected.AcademicYear}";


        message += $", but found {expected.ActualCountFound}. ";

        return message;
    }
}