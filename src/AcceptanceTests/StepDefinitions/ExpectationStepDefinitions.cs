using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
public class ExpectationStepDefinitions
{
    private static List<ExpectedPayments> _expectedPayments = new List<ExpectedPayments>();

    public static void AddExpectedPayment(ExpectedPayments expectedPayments)
    {
        _expectedPayments.Add(expectedPayments);
    }

    [BeforeScenario]
    public void BeforeEachScenario()
    {
        _expectedPayments = new List<ExpectedPayments>();
    }

    [AfterScenario]
    public async Task AfterEachScenario()
    {
        if (!_expectedPayments.Any())
            return;

        var processedEvents = new List<PaymentsGeneratedEvent>();

        await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Except(processedEvents).Any(e =>
        {
            //  Inspect event to see if it contains the expected payments
            processedEvents.Add(e);

            foreach (var expectedPayment in _expectedPayments)
            {
                if (!ValidateExpectedPayments(e, expectedPayment.Amount, expectedPayment.Count, out int actualCount))
                {
                    expectedPayment.ActualCountFound = actualCount;
                    return false;
                }
                else
                {
                    expectedPayment.Found = true;
                }
            }

            return true;
        }),
        () =>
        {
            //  Generate message to show failure reason
            if (!processedEvents.Any())
                return "Expected to find PaymentsGeneratedEvent, but none where received";

            var failmessage = string.Empty;

            foreach (var expectedPayment in _expectedPayments.Where(x => !x.Found))
            {
                failmessage += $"Expected {expectedPayment.Count} payments of {expectedPayment.Amount}, but found {expectedPayment.ActualCountFound}. ";
            }

            return failmessage;
        });
    }

    [Then(@"there are (.*) payments of (.*)")]
    public static void AddExpectedPayments(int paymentCount, decimal paymentAmount)
    {
        _expectedPayments.Add(new ExpectedPayments { Amount = paymentAmount, Count = paymentCount });
    }

    private static bool ValidateExpectedPayments(
    PaymentsGeneratedEvent paymentsGeneratedEvent, decimal? expectedAmount, int expectedNumberOfPayments, out int actualCount)
    {
        var matchingPayments = new List<Payment>();
            
        if(expectedAmount != null)
        {
            matchingPayments.AddRange(paymentsGeneratedEvent.Payments.Where(p => p.Amount == expectedAmount));
        }


        actualCount = matchingPayments.Count();

        if (actualCount != expectedNumberOfPayments)
        {
            return false;
        }

        return true;
    }

}

public class ExpectedPayments
{
    public decimal? Amount { get; set; }
    public int Count { get; set; }
    public bool Found { get; set; }
    public int ActualCountFound { get; set; }

}