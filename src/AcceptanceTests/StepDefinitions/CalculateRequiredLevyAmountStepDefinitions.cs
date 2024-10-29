using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Calculate Required Levy Amount")]
public class CalculateRequiredLevyAmountStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestContext _testContext;

    public CalculateRequiredLevyAmountStepDefinitions(ScenarioContext scenarioContext, TestContext testContext)
    {
        _scenarioContext = scenarioContext;
        _testContext = testContext;
    }

    [Given(@"an apprentice record has been approved by both the training provider & employer")]
    public async Task GivenAnApprenticeRecordHasBeenApprovedByBothTheTrainingProviderEmployer()
    {
        var inboundEvent = _testContext.Fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        inboundEvent.CourseCode = _testContext.Fixture.Create<short>().ToString();
        inboundEvent.CollectionYear = 1718;
        inboundEvent.ApprenticeshipEarning.Uln = new Random().Next(10000000, 99999999);

        _scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)] = inboundEvent;

        await _testContext.FinalisedOnProgammeLearningPaymentSendOnlyEndpoint.Publish(inboundEvent);
    }

    [When(@"the associated data is used to generate a payment")]
    public static void WhenTheAssociatedDataIsUsedToGenerateAPayment()
    {
        // intentionally left blank
    }

    [Then(@"the CalculateRequiredLevyAmount event is published to Payments v2")]
    public async Task ThenTheCalculateRequiredLevyAmountEventIsPublishedToPaymentsV2()
    {
        await WaitHelper.WaitForIt(() => CalculateOnProgrammePaymentEventHandler.ReceivedEvents.Any(CalculateOnProgrammePaymentExpectation), "Failed to find published event");
    }

    private bool CalculateOnProgrammePaymentExpectation(CalculateOnProgrammePayment outboundEvent)
    {
        var inboundEvent = (FinalisedOnProgammeLearningPaymentEvent)_scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)];

        if (outboundEvent.ApprenticeshipId != inboundEvent.Apprenticeship.ApprovalsApprenticeshipId) return false;

        outboundEvent.AccountId.Should().Be(inboundEvent.EmployerDetails.EmployingAccountId);
        outboundEvent.ActualEndDate.Should().Be(null);
        outboundEvent.AgreedOnDate.Should().Be(null);
        outboundEvent.AmountDue.Should().Be(inboundEvent.Amount);
        outboundEvent.ApprenticeshipEmployerType.Should().Be((inboundEvent.ApprenticeshipEmployerType == EmployerType.Levy ? ApprenticeshipEmployerType.Levy : ApprenticeshipEmployerType.NonLevy));
        outboundEvent.ApprenticeshipPriceEpisodeId.Should().Be(null);
        outboundEvent.CollectionPeriod.AcademicYear.Should().Be(inboundEvent.CollectionYear);
        outboundEvent.CompletionStatus.Should().Be(1); // ongoing
        outboundEvent.DeliveryPeriod.Should().Be(inboundEvent.ApprenticeshipEarning.DeliveryPeriod);
        outboundEvent.InstalmentAmount.Should().Be(inboundEvent.ApprenticeshipEarning.DeliveryPeriodAmount);
        outboundEvent.Learner.ReferenceNumber.Should().HaveLength(8).And.NotBeNullOrEmpty();
        outboundEvent.Learner.Uln.Should().Be(inboundEvent.ApprenticeshipEarning.Uln);
        outboundEvent.LearningAim.FrameworkCode.Should().Be(0);
        outboundEvent.LearningAim.FundingLineType.Should().Be(inboundEvent.ApprenticeshipEarning.FundingLineType);
        outboundEvent.LearningAim.PathwayCode.Should().Be(0);
        outboundEvent.LearningAim.ProgrammeType.Should().Be(25);
        outboundEvent.LearningAim.Reference.Should().Be("ZPROG001");
        outboundEvent.LearningAim.SequenceNumber.Should().Be(0);
        outboundEvent.LearningAim.StandardCode.Should().Be(Convert.ToInt32(inboundEvent.CourseCode));
        outboundEvent.LearningAim.StartDate.Should().Be(inboundEvent.Apprenticeship.StartDate);
        outboundEvent.LearningStartDate.Should().Be(inboundEvent.Apprenticeship.StartDate);
        outboundEvent.NumberOfInstalments.Should().Be(inboundEvent.ApprenticeshipEarning.NumberOfInstalments);
        outboundEvent.OnProgrammeEarningType.Should().Be(OnProgrammeEarningType.Learning);
        outboundEvent.PlannedEndDate.Should().Be(inboundEvent.ApprenticeshipEarning.PlannedEndDate);
        outboundEvent.PriceEpisodeIdentifier.Should().Be("");
        outboundEvent.SfaContributionPercentage.Should().Be(inboundEvent.ApprenticeshipEarning.GovernmentContributionPercentage);
        outboundEvent.StartDate.Should().Be(inboundEvent.Apprenticeship.StartDate);
        outboundEvent.TransferSenderAccountId.Should().Be(inboundEvent.EmployerDetails.FundingAccountId);
        outboundEvent.Ukprn.Should().Be(inboundEvent.ApprenticeshipEarning.ProviderIdentifier);


        return true;
    }
}