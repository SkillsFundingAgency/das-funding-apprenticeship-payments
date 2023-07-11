@EarningsGeneratedEndpoint
@ReleasePaymentsEndpoint
@PaymentsGeneratedEndpoint
@FinalisedOnProgammeLearningPaymentSendOnlyEndpoint

Feature: Payments Release

Scenario: Payments Release
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	And payments are generated with the correct learning amounts
	And payments are released
	Then the correct payments are released