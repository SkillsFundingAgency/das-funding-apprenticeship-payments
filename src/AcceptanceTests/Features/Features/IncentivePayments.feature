Feature: Incentive Payments

These tests verify that incentive payments are correctly handled

Scenario: recalculated payments have relevant incentive payments removed
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmenType     |
	| 2024 | 9     | 200    | OnProgramme       |
	| 2024 | 9     | 500    | ProviderIncentive |
	| 2024 | 9     | 500    | EmployerIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	| 2024 | 12    | 500    | EmployerIncentive |
	And payments are calculated
	And the date is now 2024-11-01
	And payments are released
	When payments are recalculated with the following earnings
	| Year | Month | Amount | InstalmenType     |
	| 2024 | 9     | 200    | OnProgramme       |
	| 2024 | 9     | 500    | ProviderIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	Then new payments are generated with the following amounts
	| Year | Month | Amount |
	| 2425 | 9     | 200    |
	| 2425 | 9     | 500    |
	| 2425 | 9     | 500    |
	| 2425 | 9     | -500   |
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 500    |


# Before FLP-1036 InstalmentType and PaymentType fields did not exist in the database
# Under FLP-1036 these fields have been added to accomodate the new payment types
# therefore it can be assumed that any records where these values are null should be treated as onprogramme
# this test is to ensure that the old records are treated as onprogramme
Scenario: Treat old records with no instalmentType set as onprogramme
	Given there is an apprenticeship with instalment and payment types set to null
	And the apprenticeship has previously released payments
	When earnings are regenerated
	Then the payment type for previously paid payments is set to onprogramme
