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