Feature: Calculate payments for earnings

Scenario: Simple Payments Generation
	Given earnings have been generated
	And all of the earnings are due in the future
	And no payments have previously been generated
	When payments are calculated
	Then Payments are generated with the correct learning amounts