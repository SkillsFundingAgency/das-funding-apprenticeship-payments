using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Recalculate payments for earnings")]
public class PaymentsRecalculationStepDefinitions
{
	private readonly ScenarioContext _scenarioContext;
	private readonly TestContext _testContext;
	private static Guid _apprenticeshipKey;
	private static List<ExpectedPayments> _expectedPayments = new List<ExpectedPayments>();
	private static int _expectedNumberOfEventsPublished = 0;
	private static EarningsGeneratedEvent _previousEarningsGeneratedEvent;
	private static ApprenticeshipEarningsRecalculatedEvent _earningsRecalculatedEvent;

	public PaymentsRecalculationStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
	{
		_scenarioContext = scenarioContext;
		_testContext = testContext;
	}

	[BeforeScenario]
	public void BeforeEachScenario()
	{
		_expectedPayments = new List<ExpectedPayments>();
		_expectedNumberOfEventsPublished = 0;
		PaymentsGeneratedEventHandler.ReceivedEvents.Clear();
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

			foreach (var expectedPayment in _expectedPayments.Where(x=> !x.Found))
			{
				failmessage += $"Expected {expectedPayment.Count} payments of {expectedPayment.Amount}, but found {expectedPayment.ActualCountFound}. ";
			}

			return failmessage; 
		});
	}

	[Given(@"some previous earnings have been paid")]
	public async Task GivenSomePreviousEarningsHaveBeenPaid()
	{
		var periods = new List<DeliveryPeriod>
		{
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.Month, (short)DateTime.Now.Year, 1000),
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(1).Month, (short)DateTime.Now.AddMonths(1).Year, 1000)  // next month not paid yet
		};

		await GenerateExistingPayments(periods);
	}

	[Given(@"there are (.*) payments of (.*), which started (.*) months ago")]
	public async Task GivenAYearsPreviousEarningsHaveBeenPaid(int totalNumberOfPayments, decimal paymentAmount, int monthsAgo)
	{
		var offsetMonths = monthsAgo - 1;// to account for the current month
        var startDate = DateTime.Now.AddMonths(-offsetMonths);
        var periods = new List<DeliveryPeriod>();

		for (var i = 0; i < totalNumberOfPayments; i++)
        {
            periods.Add(PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(i).Month, (short)DateTime.Now.AddMonths(i).Year, 1000));
		}

        await GenerateExistingPayments(periods);
	}

	[Given(@"recalculated earnings are generated")]
    public async Task RecalculatedEarningsHaveBeenGenerated()
    {
        //build event for recalculated earnings
        var periods = new List<DeliveryPeriod>
        {
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.Month, (short)DateTime.Now.Year, 1200), //this month already paid
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(1).Month, (short)DateTime.Now.AddMonths(1).Year, 1200) // next month not paid yet
        };

		await GenerateRecalculatedEarnings(periods);
	}

	[Given(@"recalculated earnings now have (.*) payments of (.*), which started (.*) months ago")]
	public async Task RecalculatedEarningsHaveBeenGeneratedWithAnEarlierStartDate(int totalNumberOfPayments, decimal paymentAmount, int monthsAgo)
	{
		var offsetMonths = monthsAgo - 1;// to account for the current month
		var startDate = DateTime.Now.AddMonths(-offsetMonths);
		var periods = new List<DeliveryPeriod>();

		for (var i = 0; i < totalNumberOfPayments; i++)
		{
			periods.Add(PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.AddMonths(i).Month, (short)DateTime.Now.AddMonths(i).Year, paymentAmount));
		}

		await GenerateRecalculatedEarnings(periods);
	}

	[When("payments are recalculated")]
    public static async Task PaymentsAreRecalculated()
    {
		await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Count == _expectedNumberOfEventsPublished, 
			$"Failed to receive expected number of events Received: {PaymentsGeneratedEventHandler.ReceivedEvents.Count} Expected: {_expectedNumberOfEventsPublished}");
	}

    [Then("new payments are generated with the correct learning amounts")]
    public static async Task NewPaymentsAreGeneratedWithTheCorrectLearningAmounts()
    {
        await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Any(e =>
            {
                return e.ApprenticeshipKey == _apprenticeshipKey
                       && e.Payments.Count == 3
                       && e.Payments.Any(x => x.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod() && x.Amount == 1000) //original payment
                       && e.Payments.Any(x => x.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod() && x.Amount == 200) //diff payment
                       && e.Payments.Any(x => x.CollectionPeriod == ((byte)DateTime.Now.AddMonths(1).Month).ToDeliveryPeriod() && x.Amount == 1200); }), //payment for month not yet sent
            "Failed to find published PaymentsGenerated event for recalculated payments");
    }

	[Then(@"there are (.*) payments of (.*)")]
	public static void AddExpectedPayments(int paymentCount, decimal paymentAmount)
	{
		_expectedPayments.Add(new ExpectedPayments { Amount = paymentAmount, Count = paymentCount });
	}

	private async Task GenerateExistingPayments(List<DeliveryPeriod> periods)
	{
		_apprenticeshipKey = Guid.NewGuid();

		_previousEarningsGeneratedEvent = _testContext.Fixture
			.Build<EarningsGeneratedEvent>()
			.With(x => x.DeliveryPeriods, periods)
			.With(x => x.Uln, _testContext.Fixture.Create<int>().ToString())
			.With(x => x.TrainingCode, _testContext.Fixture.Create<int>().ToString())
			.With(x => x.ApprenticeshipKey, _apprenticeshipKey)
			.Create();

		_scenarioContext[ContextKeys.EarningsGeneratedEvent] = _previousEarningsGeneratedEvent;

		//publish event for previous earnings
		await _testContext.EarningsGeneratedEndpoint.Publish(_previousEarningsGeneratedEvent);

		//wait for payments to be generated
		await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Any(e =>
			e.ApprenticeshipKey == _apprenticeshipKey
			&& e.Payments.Count == periods.Count), "Failed to find published PaymentsGenerated event for previously generated payments");

		//release payments for this month
		var releasePaymentsCommand = new ReleasePaymentsCommand
		{
			CollectionPeriod = ((byte)DateTime.Now.Month).ToDeliveryPeriod(),
			CollectionYear = ((short)DateTime.Now.Year).ToAcademicYear((byte)DateTime.Now.Month)
		};
		await _testContext.ReleasePaymentsEndpoint.Publish(releasePaymentsCommand);

		//wait for finalised on programme learning payment event to be published
		await WaitHelper.WaitForIt(() =>
		{
			return FinalisedOnProgammeLearningPaymentEventHandler.ReceivedEvents.Any(e =>
				e.CollectionPeriod == releasePaymentsCommand.CollectionPeriod
				&& e.ApprenticeshipKey == _apprenticeshipKey);
		}, "Failed to find published FinalisedOnProgammeLearningPaymentEvent for previously generated payment");

		_expectedNumberOfEventsPublished++;
	}

	private async Task GenerateRecalculatedEarnings(List<DeliveryPeriod> periods)
	{
		//build event for recalculated earnings
		_earningsRecalculatedEvent = _testContext.Fixture
			.Build<ApprenticeshipEarningsRecalculatedEvent>()
			.With(x => x.DeliveryPeriods, periods)
			.With(x => x.ApprenticeshipKey, _apprenticeshipKey)
			.Create();

		_scenarioContext[ContextKeys.EarningsRecalculatedEvent] = _earningsRecalculatedEvent;

		//publish event for recalculated earnings
		await _testContext.EarningsRecalculatedEndpoint.Publish(_earningsRecalculatedEvent);
		_expectedNumberOfEventsPublished++;
	}

	private static bool ValidateExpectedPayments(
		PaymentsGeneratedEvent paymentsGeneratedEvent, decimal expectedAmount, int expectedNumberOfPayments, out int actualCount)
	{
		actualCount = paymentsGeneratedEvent.Payments.Where(p => p.Amount == expectedAmount).Count();

		if (actualCount != expectedNumberOfPayments)
		{
			return false;
		}

		return true;
	}

}

public class ExpectedPayments
{
	public decimal Amount { get; set; }
	public int Count { get; set; }
	public bool Found { get; set; }
	public int ActualCountFound { get; set; }
}