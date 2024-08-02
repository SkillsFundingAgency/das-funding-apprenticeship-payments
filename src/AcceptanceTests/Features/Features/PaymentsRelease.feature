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
	Then no payments are released for this apprenticeship

Scenario: Payments Unfrozen
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	And payments are calculated
	And the payments are frozen
	And payments are generated with the correct learning amounts
	And payments are released
	And no payments are released for this apprenticeship
	When the payments are unfrozen
	And payments are released for the next collection period
	Then the correct unfrozen payments are released

Scenario: Unfrozen payments which are in the previous academic year are not released
	Given earnings have been generated
	And the earnings started in academic year 23/24 and run for 2 years
	And the date is now 2023-11-15
	And payments are calculated
	And the date is now 2023-12-15
	And the payments are frozen
	And the date is now 2024-11-15
	When the payments are unfrozen
	And payments are released
	Then their are 4 payments paid for academic year 23/24
	Then their are 3 payments paid for academic year 24/25
	