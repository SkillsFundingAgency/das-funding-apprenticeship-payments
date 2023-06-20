using AutoFixture;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Handlers;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

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
        _scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)] = inboundEvent;

        await _testContext.FinalisedOnProgammeLearningPaymentSendOnlyEndpoint.Publish(inboundEvent);
    }

    [When(@"the associated data is used to generate a payment")]
    public void WhenTheAssociatedDataIsUsedToGenerateAPayment()
    {
        // 
    }

    [Then(@"the CalculateRequiredLevyAmount event is published to Payments v2")]
    public async Task ThenTheCalculateRequiredLevyAmountEventIsPublishedToPaymentsV2()
    {
        await WaitHelper.WaitForIt(() => CalculatedRequiredLevyAmountHandler.ReceivedEvents.Any(CalculatedRequiredLevyAmountExpectation), "Failed to find published event");
    }

    private bool CalculatedRequiredLevyAmountExpectation(CalculatedRequiredLevyAmount outboundEvent)
    {
        var inboundEvent = (FinalisedOnProgammeLearningPaymentEvent)_scenarioContext[nameof(FinalisedOnProgammeLearningPaymentEvent)];

        return outboundEvent.ApprenticeshipId == inboundEvent.EmployerDetails.FundingCommitmentId
               && outboundEvent.AccountId == inboundEvent.EmployerDetails.EmployingAccountId
               && outboundEvent.ActualEndDate == null
               && outboundEvent.AgreedOnDate == null
               && outboundEvent.AgreementId == null
               && outboundEvent.AmountDue == inboundEvent.Amount
               && outboundEvent.ApprenticeshipEmployerType == 0
               && outboundEvent.ApprenticeshipId == inboundEvent.EmployerDetails.FundingCommitmentId
               && outboundEvent.ApprenticeshipPriceEpisodeId == null
               && outboundEvent.ClawbackSourcePaymentEventId == null
               && outboundEvent.CollectionPeriod.AcademicYear == inboundEvent.CollectionYear
               && outboundEvent.CompletionStatus == 1 // ongoing
               && outboundEvent.CompletionAmount == 0
               && outboundEvent.ContractType == ContractType.Act1
               && outboundEvent.DeliveryPeriod == inboundEvent.ApprenticeshipEarnings.DeliveryPeriod
               && outboundEvent.EarningEventId == inboundEvent.ApprenticeshipEarnings.ApprenticeshipEarningsId
               && outboundEvent.IlrFileName == null
               && outboundEvent.InstalmentAmount == inboundEvent.ApprenticeshipEarnings.DeliveryPeriodAmount
               && outboundEvent.Learner.ReferenceNumber == null
               && outboundEvent.Learner.Uln == inboundEvent.ApprenticeshipEarnings.Uln
               && outboundEvent.LearningAim.FrameworkCode == 0
               && outboundEvent.LearningAim.FundingLineType == inboundEvent.ApprenticeshipEarnings.FundingLineType
               && outboundEvent.LearningAim.PathwayCode == 0
               && outboundEvent.LearningAim.ProgrammeType == 0
               && outboundEvent.LearningAim.Reference == "ZPROG001"
               && outboundEvent.LearningAim.SequenceNumber == 0
               && outboundEvent.LearningAim.StandardCode == inboundEvent.CourseCode
               && outboundEvent.LearningAim.StartDate == inboundEvent.Apprenticeship.StartDate
               && outboundEvent.LearningAimSequenceNumber == 0
               && outboundEvent.LearningStartDate == inboundEvent.Apprenticeship.StartDate
               && outboundEvent.NumberOfInstalments == inboundEvent.ApprenticeshipEarnings.NumberOfInstalments
               && outboundEvent.OnProgrammeEarningType == OnProgrammeEarningType.Learning
               && outboundEvent.PlannedEndDate == inboundEvent.ApprenticeshipEarnings.PlannedEndDate
               && outboundEvent.PriceEpisodeIdentifier == ""
               && outboundEvent.Priority == 0
               && outboundEvent.ReportingAimFundingLineType == ""
               && outboundEvent.SfaContributionPercentage == inboundEvent.ApprenticeshipEarnings.GovernmentContributionPercentage
               && outboundEvent.StartDate == inboundEvent.Apprenticeship.StartDate
               && outboundEvent.TransferSenderAccountId == inboundEvent.EmployerDetails.FundingAccountId
               && outboundEvent.Ukprn == inboundEvent.ApprenticeshipEarnings.ProviderIdentifier
               && outboundEvent.TransactionType == TransactionType.Learning
            ;
    }
}