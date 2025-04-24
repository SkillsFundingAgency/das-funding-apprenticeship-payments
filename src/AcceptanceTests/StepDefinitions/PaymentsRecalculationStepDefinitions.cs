using AutoFixture;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.DataModels;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Recalculate payments for earnings")]
[Scope(Feature = "Incentive Payments")]
public class PaymentsRecalculationStepDefinitions
{
	private readonly ScenarioContext _scenarioContext;
	private readonly TestContext _testContext;
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
		_expectedNumberOfEventsPublished = 0;
        _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Clear();
	}

	[Given(@"some previous earnings have been paid")]
	public async Task GivenSomePreviousEarningsHaveBeenPaid()
	{
		var periods = new List<DeliveryPeriod>
		{
            PeriodHelper.CreateDeliveryPeriod((byte)DateTime.Now.Month, (short)DateTime.Now.Year, 1000), //this month already paid
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
            periods.Add(PeriodHelper.CreateDeliveryPeriod((byte)startDate.AddMonths(i).Month, (short)startDate.AddMonths(i).Year, paymentAmount));
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
			periods.Add(PeriodHelper.CreateDeliveryPeriod((byte)startDate.AddMonths(i).Month, (short)startDate.AddMonths(i).Year, paymentAmount));
		}

		await GenerateRecalculatedEarnings(periods);
	}

    [When(@"payments are recalculated with the following earnings")]
    public async Task PaymentsAreRecalculated(Table table)
    {
        var data = table.CreateSet<EarningsDataRow>().ToList();
        var periods = data.Select(x => PeriodHelper.CreateDeliveryPeriod(x.Month, x.Year, x.Amount, x.InstalmentType)).ToList();
        await GenerateRecalculatedEarnings(periods);
    }

    [When("payments are recalculated")]
    public async Task PaymentsAreRecalculated()
    {
		await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Count == _expectedNumberOfEventsPublished, 
			$"Failed to receive expected number of events Received: {_testContext.ReceivedEvents<PaymentsGeneratedEvent>().Count} Expected: {_expectedNumberOfEventsPublished}");
	}

    [Then("new payments are generated with the correct learning amounts")]
    public async Task NewPaymentsAreGeneratedWithTheCorrectLearningAmounts()
    {
		var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];

        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Any(e =>
            {
                return e.ApprenticeshipKey == apprenticeshipKey
                       && e.Payments.Count == 3
                       && e.Payments.Any(x => x.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod() && x.Amount == 1000) //original payment
                       && e.Payments.Any(x => x.CollectionPeriod == ((byte)DateTime.Now.Month).ToDeliveryPeriod() && x.Amount == 200) //diff payment
                       && e.Payments.Any(x => x.CollectionPeriod == ((byte)DateTime.Now.AddMonths(1).Month).ToDeliveryPeriod() && x.Amount == 1200); }), //payment for month not yet sent
            "Failed to find published PaymentsGenerated event for recalculated payments");
    }

    [Then("new payments are generated with the following amounts")]
    public async Task NewPaymentsAreGeneratedWithTheCorrectLearningAmounts(Table table)
    {
        var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];
        var expectedData = table.CreateSet<ExpectedPaymentDataRow>().ToList();

        await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Any(e =>
        {
			return e.ApprenticeshipKey == apprenticeshipKey
                   && e.Payments.Count == expectedData.Count
				   && expectedData.All(x => e.Payments.Any(p => p.CollectionYear == x.Year && p.DeliveryPeriod == x.Month.ToDeliveryPeriod() && p.Amount == x.Amount));
        }), 
            "Failed to find published PaymentsGenerated event for recalculated payments");
    }


    private async Task GenerateExistingPayments(List<DeliveryPeriod> periods)
	{
		var apprenticeshipKey = Guid.NewGuid();
        _scenarioContext["apprenticeshipKey"] = apprenticeshipKey;
        var uln = _testContext.Fixture.Create<long>();
		_testContext.Ulns.Add(uln);

		_previousEarningsGeneratedEvent = _testContext.Fixture
			.Build<EarningsGeneratedEvent>()
			.With(x => x.DeliveryPeriods, periods)
			.With(x => x.Uln, uln.ToString())
			.With(x => x.TrainingCode, _testContext.Fixture.Create<int>().ToString())
			.With(x => x.ApprenticeshipKey, apprenticeshipKey)
			.Create();

		_scenarioContext[ContextKeys.EarningsGeneratedEvent] = _previousEarningsGeneratedEvent;

        //publish event for previous earnings
        await _testContext.TestFunction!.PublishEvent(_previousEarningsGeneratedEvent);

		//wait for payments to be generated
		await WaitHelper.WaitForIt(() => _testContext.ReceivedEvents<PaymentsGeneratedEvent>().Any(e =>
			e.ApprenticeshipKey == apprenticeshipKey
			&& e.Payments.Count == periods.Count), "Failed to find published PaymentsGenerated event for previously generated payments");

		//release payments for this month
        var currentYear = ((short)DateTime.Now.Year).ToAcademicYear((byte)DateTime.Now.Month);
        await ReleasePayments(((byte)DateTime.Now.Month).ToDeliveryPeriod(), currentYear);

		//release payments for any previous years
        var previousYears = periods.Where(x => x.AcademicYear < currentYear).Select(x => x.AcademicYear).Distinct();
        foreach (var previousYear in previousYears)
        {
            await ReleasePayments(14, previousYear);
        }

        _expectedNumberOfEventsPublished++;
	}

    private async Task ReleasePayments(byte collectionPeriod, short collectionYear)
    {
        await _testContext.TestFunction!.PostReleasePayments(collectionYear, collectionPeriod);
        await _testContext.TestFunction.WaitUntilOrchestratorComplete(nameof(ReleasePaymentsOrchestrator));
    }

    private async Task GenerateRecalculatedEarnings(List<DeliveryPeriod> periods)
	{
        var apprenticeshipKey = (Guid)_scenarioContext["apprenticeshipKey"];

        //build event for recalculated earnings
        _earningsRecalculatedEvent = _testContext.Fixture
			.Build<ApprenticeshipEarningsRecalculatedEvent>()
			.With(x => x.DeliveryPeriods, periods)
			.With(x => x.ApprenticeshipKey, apprenticeshipKey)
			.Create();

		_scenarioContext[ContextKeys.EarningsRecalculatedEvent] = _earningsRecalculatedEvent;

        //publish event for recalculated earnings
        await _testContext.TestFunction.PublishEvent(_earningsRecalculatedEvent);
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