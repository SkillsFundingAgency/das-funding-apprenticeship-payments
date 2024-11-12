@EarningsGeneratedEndpoint
@ReleasePaymentsEndpoint
@PaymentsGeneratedEndpoint
@FinalisedOnProgammeLearningPaymentEndpoint
@FreezePaymentsEndpoint
@UnfreezePaymentsEndpoint
@ResetSentForPaymentFlagEndpoint

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

Scenario: Unfrozen payments which are in the previous academic year and year is hard closed are not released
	Given earnings have been generated
	And the earnings started in academic year 23/24 and run for 2 years
	And the date is now 2023-11-15
	And payments are calculated
	And payments are released
	And the date is now 2023-12-15
	And the payments are frozen
	And payments are released every month until 2024-10-15
	And the date is now 2024-11-15
	When the payments are unfrozen
	And payments are released
	Then for academic year 23/24 there are 4 payments of 1000 released
	Then for academic year 24/25 there are 4 payments of 1000 released	

Scenario: Unfrozen payments which are in the previous academic year but is not hard closed are released
	Given earnings have been generated
	And the earnings started in academic year 23/24 and run for 2 years
	And the date is now 2023-11-15
	And payments are calculated
	And payments are released
	And the date is now 2023-12-15
	And the payments are frozen
	And payments are released every month until 2024-09-15
	And the date is now 2024-10-15
	When the payments are unfrozen
	And payments are released
	Then for academic year 23/24 there are 4 payments of 1000 released
	Then for academic year 24/25 there are 11 payments of 1000 released
	# the reason 11 payments are released in 24/25 is because although they were originally intended 23/24 they were actually released in 24/25

Scenario: Payments Not Released Multiple Times
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	And payments are generated with the correct learning amounts
	And payments are released
	And the correct payments are released
	And payments are released again
	Then multiple copies of the same payment are not released

Scenario: Payments Released Again If Reset
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	And payments are generated with the correct learning amounts
	And payments are released
	And the correct payments are released
	And ResetSentForPaymentFlagForCollectionPeriod has been triggered
	And payments are released again
	Then the correct payments are re-released