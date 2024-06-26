@EarningsGeneratedEndpoint
@ReleasePaymentsEndpoint
@PaymentsGeneratedEndpoint
@FinalisedOnProgammeLearningPaymentEndpoint
@FreezePaymentsEndpoint
@UnfreezePaymentsEndpoint

Feature: Payments Release

Scenario: Payments Release
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	And payments are generated with the correct learning amounts
	And payments are released
	Then the correct payments are released

Scenario: Payments Frozen
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	And the payments are frozen
	And payments are generated with the correct learning amounts
	And payments are released
	Then no payments are released

Scenario: Payments Unfrozen
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	And payments are calculated
	And the payments are frozen
	And payments are generated with the correct learning amounts
	And payments are released
	And no payments are released
	When the payments are unfrozen
	And payments are released for the next collection period
	Then the correct unfrozen payments are released
	