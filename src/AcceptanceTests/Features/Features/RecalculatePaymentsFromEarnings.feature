@EarningsGeneratedEndpoint
@PaymentsGeneratedEndpoint
@ReleasePaymentsEndpoint
@EarningsRecalculatedEndpoint
@FinalisedOnProgammeLearningPaymentEndpoint

Feature: Recalculate payments for earnings

Scenario: Payments Recalculation
	Given some previous earnings have been paid
	And recalculated earnings are generated
	When payments are recalculated
	Then new payments are generated with the correct learning amounts

Scenario: Payments Recalculation when start date is changed to ealier date
	Given there are 20 payments of 600, which started 8 months ago
	And recalculated earnings now have 22 payments of 545.45, which started 10 months ago
	When payments are recalculated
	Then there are 8 payments of 600
	Then there are 14 payments of 545.45
	Then there are 8 payments of -54.55

Scenario: Payments Recalculation when start date is changed to later date
	Given there are 20 payments of 620, which started 10 months ago
	And recalculated earnings now have 17 payments of 729.41, which started 7 months ago
	When payments are recalculated
	Then there are 10 payments of 620
	Then there are 10 payments of 729.41
	Then there are 7 payments of 109.41
	Then there are 3 payments of -620