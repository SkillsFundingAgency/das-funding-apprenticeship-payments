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
    private static EarningsGeneratedEvent _previousEarningsGeneratedEvent;
    private static ApprenticeshipEarningsRecalculatedEvent _earningsRecalculatedEvent;

    public PaymentsRecalculationStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [Given(@"some previous earnings have been paid")]
    public async Task GivenSomePreviousEarningsHaveBeenPaid()
    {
        var periods = new List<DeliveryPeriod>
        {
            new() { CalenderYear = (short)DateTime.Now.Year, CalendarMonth = (byte)DateTime.Now.Month, LearningAmount = 1000 }, //this month already paid
            new() { CalenderYear = (short)DateTime.Now.AddMonths(1).Year, CalendarMonth = (byte)DateTime.Now.AddMonths(1).Month, LearningAmount = 1000 } // next month not paid yet
        };

		await GenerateExistingPayments(periods);
    }

	[Given(@"there are (.*) payments of (.*), which started (.*) months ago")]
	public async Task GivenAYearsPreviousEarningsHaveBeenPaid(int totalNumberOfPayments, decimal paymentAmount, int months)
	{
		var offsetMonths = months - 1;// to account for the current month
        var originalStartDate = DateTime.Now.AddMonths(-offsetMonths);
        var periods = new List<DeliveryPeriod>();

		for (var i = 0; i < totalNumberOfPayments; i++)
        {
            periods.Add(new() { CalenderYear = (short)originalStartDate.AddMonths(i).Year, CalendarMonth = (byte)originalStartDate.AddMonths(i).Month, LearningAmount = paymentAmount });
		}

        await GenerateExistingPayments(periods);
	}

	[Given(@"recalculated earnings are generated")]
    public async Task RecalculatedEarningsHaveBeenGenerated()
    {
        //build event for recalculated earnings
        var periods = new List<DeliveryPeriod>
        {
            new() { CalenderYear = (short)DateTime.Now.Year, CalendarMonth = (byte)DateTime.Now.Month, LearningAmount = 1200 }, //this month already paid
            new() { CalenderYear = (short)DateTime.Now.AddMonths(1).Year, CalendarMonth = (byte)DateTime.Now.AddMonths(1).Month, LearningAmount = 1200 } // next month not paid yet
        };

		await GenerateRecalculatedEarnings(periods);
	}

	[Given(@"recalculated earnings now have (.*) payments of (.*), which started (.*) months ago")]
	public async Task RecalculatedEarningsHaveBeenGeneratedWithAnEarlierStartDate(int totalNumberOfPayments, decimal paymentAmount, int months)
	{
		var offsetMonths = months - 1;// to account for the current month
		var originalStartDate = DateTime.Now.AddMonths(-offsetMonths);
		var periods = new List<DeliveryPeriod>();

		for (var i = 0; i < totalNumberOfPayments; i++)
		{
			periods.Add(new() { CalenderYear = (short)originalStartDate.AddMonths(i).Year, CalendarMonth = (byte)originalStartDate.AddMonths(i).Month, LearningAmount = paymentAmount });
		}

		await GenerateRecalculatedEarnings(periods);
	}

	[When("payments are recalculated")]
    public static void PaymentsAreRecalculated()
    {
        //intentionally left blank
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

	[Then(@"new payments are generated with (.*) payments of (.*), (.*) payments of (.*) and (.*) payments of (.*)")]
	public static async Task NewPaymentsAreGeneratedWithTheCorrectLearningAmountsForAnEarlierStartDate(
		int paymentCount1, decimal paymentAmount1, int paymentCount2, decimal paymentAmount2, int paymentCount3, decimal paymentAmount3)
	{
		var failmessage = "but no events published";
		var processedEvents = new List<PaymentsGeneratedEvent>();

		await WaitHelper.WaitForIt(() => PaymentsGeneratedEventHandler.ReceivedEvents.Except(processedEvents).Any(e =>
		{
			processedEvents.Add(e);

			if (!ValidateExpectedPayments(e, paymentAmount1, paymentCount1, ref failmessage))
				return false;

			if (!ValidateExpectedPayments(e, paymentAmount2, paymentCount2, ref failmessage))
				return false;

			if (!ValidateExpectedPayments(e, paymentAmount3, paymentCount3, ref failmessage))
				return false;

			return true;
		}), 
			() => { return $"Expected to find PaymentsGenerated {failmessage}"; } );


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

		_previousEarningsGeneratedEvent.SetDeliveryPeriodsAccordingToCalendarMonths();

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
	}

	private async Task GenerateRecalculatedEarnings(List<DeliveryPeriod> periods)
	{
		//build event for recalculated earnings
		_earningsRecalculatedEvent = _testContext.Fixture
			.Build<ApprenticeshipEarningsRecalculatedEvent>()
			.With(x => x.DeliveryPeriods, periods)
			.With(x => x.ApprenticeshipKey, _apprenticeshipKey)
			.Create();

		_earningsRecalculatedEvent.SetDeliveryPeriodsAccordingToCalendarMonths();

		_scenarioContext[ContextKeys.EarningsRecalculatedEvent] = _earningsRecalculatedEvent;

		//publish event for recalculated earnings
		await _testContext.EarningsRecalculatedEndpoint.Publish(_earningsRecalculatedEvent);
	}

	private static bool ValidateExpectedPayments(
		PaymentsGeneratedEvent paymentsGeneratedEvent, decimal expectedAmount, int expectedNumberOfPayments, ref string failmessage)
	{
		var numberOfPayments = paymentsGeneratedEvent.Payments.Where(p => p.Amount == expectedAmount).Count();

		if (numberOfPayments != expectedNumberOfPayments)
		{
			failmessage = $"with {expectedNumberOfPayments} payments of {expectedAmount} but found {numberOfPayments}";
			return false;
		}

		return true;
	}
}