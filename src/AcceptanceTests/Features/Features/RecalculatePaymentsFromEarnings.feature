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

Scenario: Payments Recalculation when start date is changed to earlier date
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

Scenario: Multiple Payments of the same type in a delivery period
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType  |
	And payments are calculated
	When payments are recalculated with the following earnings
	| Year | Month | Amount | InstalmentType  |
	| 2024 | 9     | 300    | MathsAndEnglish |
	| 2024 | 9     | 100    | MathsAndEnglish |
	Then new payments are generated with the following amounts
	| AcademicYear | Month | Amount |
	| 2425         | 9     | 300    |
	| 2425         | 9     | 100    |
