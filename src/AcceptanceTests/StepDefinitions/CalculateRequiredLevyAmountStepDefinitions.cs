using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class CalculateRequiredLevyAmountStepDefinitions
    {
        [Given(@"an apprentice record has been approved by both the training provider & employer")]
        public void GivenAnApprenticeRecordHasBeenApprovedByBothTheTrainingProviderEmployer()
        {
            throw new PendingStepException();
        }

        [When(@"the associated data is used to generate a payment")]
        public void WhenTheAssociatedDataIsUsedToGenerateAPayment()
        {
            throw new PendingStepException();
        }

        [Then(@"the CalculateRequiredLevyAmount event is published to Payments v(.*)")]
        public void ThenTheCalculateRequiredLevyAmountEventIsPublishedToPaymentsV(int p0)
        {
            throw new PendingStepException();
        }
    }
}
