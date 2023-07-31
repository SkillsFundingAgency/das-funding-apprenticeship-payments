@EarningsGeneratedEndpoint
@PaymentsGeneratedEndpoint
@ReleasePaymentsEndpoint

Feature: Calculate payments for earnings

Scenario: Simple Payments Generation
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	Then payments are generated with the correct learning amounts

Scenario: Rolled up Payments Generation
	Given earnings have been generated
	And two of the earnings are due in a past month
	And no payments have previously been generated
	When payments are calculated
	Then the past earnings are allocated to the current month

