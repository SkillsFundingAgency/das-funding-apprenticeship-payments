@FinalisedOnProgammeLearningPaymentSendOnlyEndpoint
@CalculatedRequiredLevyAmountEndpoint
Feature: Calculate Required Levy Amount

As the DfE 
I want to provide sufficient flexible payment data to Payments v2
So that payments can be generated

Scenario: Publish CalculateRequiredLevyAmount event
    Given an apprentice record has been approved by both the training provider & employer
    When the associated data is used to generate a payment
    Then the CalculateRequiredLevyAmount event is published to Payments v2
