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


Scenario: Payments Recalculation when start date changes
	Given a years previous earnings have been paid
	And recalculated earnings are generated with an earlier start date
	When payments are recalculated
	Then new payments are generated with the correct learning amounts for an earlier start date