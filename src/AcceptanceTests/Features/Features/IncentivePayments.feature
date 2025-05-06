Feature: Incentive Payments

These tests verify that incentive payments are correctly handled

Scenario: recalculated payments have relevant incentive payments removed
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType     |
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
	| Year | Month | Amount | InstalmentType     |
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
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 500    |

Scenario: recalculated payments have relevant incentive payments added
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType     |
	| 2024 | 9     | 200    | OnProgramme       |
	| 2024 | 9     | 500    | ProviderIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	And payments are calculated
	And the date is now 2024-11-01
	And payments are released
	When payments are recalculated with the following earnings
	| Year | Month | Amount | InstalmentType     |
	| 2024 | 9     | 200    | OnProgramme       |
	| 2024 | 9     | 500    | ProviderIncentive |
	| 2024 | 9     | 500    | EmployerIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	| 2024 | 12    | 500    | EmployerIncentive |
	Then new payments are generated with the following amounts
	| Year | Month | Amount |
	| 2425 | 9     | 200    |
	| 2425 | 9     | 500    |
	| 2425 | 9     | 500    |
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 500    |
	| 2425 | 12    | 500    |

Scenario: recalculated payments have relevant incentive payments adjusted
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType     |
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
	| Year | Month | Amount | InstalmentType     |
	| 2024 | 9     | 200    | OnProgramme       |
	| 2024 | 9     | 600    | ProviderIncentive |
	| 2024 | 9     | 400    | EmployerIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 600    | ProviderIncentive |
	| 2024 | 12    | 400    | EmployerIncentive |
	Then new payments are generated with the following amounts
	| Year | Month | Amount |
	| 2425 | 9     | 200    |
	| 2425 | 9     | 600    |
	| 2425 | 9     | 400    |
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 600    |
	| 2425 | 12    | 400    |


# Before FLP-1036 InstalmentType and PaymentType fields did not exist in the database
# Under FLP-1036 these fields have been added to accomodate the new payment types
# therefore it can be assumed that any records where these values are null should be treated as onprogramme
# this test is to ensure that the old records are treated as onprogramme
Scenario: Treat old records with no instalmentType set as onprogramme
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType     |
	| 2024 | 9     | 200    |                   |
	| 2024 | 9     | 500    | ProviderIncentive |
	| 2024 | 9     | 500    | EmployerIncentive |
	| 2024 | 10    | 200    |                   |
	| 2024 | 11    | 200    |                   |
	| 2024 | 12    | 200    |                   |
	| 2024 | 12    | 500    | ProviderIncentive |
	| 2024 | 12    | 500    | EmployerIncentive |
	And payments are calculated
	And the date is now 2024-11-01
	And payments are released
	When payments are recalculated with the following earnings
	| Year | Month | Amount | InstalmentType     |
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
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 500    |


Scenario: Payments are not generated for incentives in a hard closed year
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType     |
	| 2023 | 9     | 200    | OnProgramme       |
	| 2023 | 9     | 500    | ProviderIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	And payments are calculated
	And the date is now 2024-11-01
	And payments are released
	When payments are recalculated with the following earnings
	| Year | Month | Amount | InstalmentType     |
	| 2023 | 9     | 200    | OnProgramme       |
	| 2023 | 9     | 500    | ProviderIncentive |
	| 2024 | 9     | 500    | EmployerIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	| 2024 | 12    | 500    | EmployerIncentive |
	Then new payments are generated with the following amounts
	| Year | Month | Amount |
	| 2425 | 9     | 500    |
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 500    |
	| 2425 | 12    | 500    |

Scenario: Payments in a previous year are generated because not hardClosed
	Given the date is now 2024-09-01
	And The following earnings are generated
	| Year | Month | Amount | InstalmentType     |
	| 2023 | 9     | 200    | OnProgramme       |
	| 2023 | 9     | 500    | ProviderIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	And payments are calculated
	And the date is now 2024-10-01
	And payments are released
	When payments are recalculated with the following earnings
	| Year | Month | Amount | InstalmentType     |
	| 2023 | 9     | 200    | OnProgramme       |
	| 2023 | 9     | 500    | ProviderIncentive |
	| 2024 | 9     | 500    | EmployerIncentive |
	| 2024 | 10    | 200    | OnProgramme       |
	| 2024 | 11    | 200    | OnProgramme       |
	| 2024 | 12    | 200    | OnProgramme       |
	| 2024 | 12    | 500    | ProviderIncentive |
	| 2024 | 12    | 500    | EmployerIncentive |
	Then new payments are generated with the following amounts
	| Year | Month | Amount |
	| 2324 | 9     | 200    |
	| 2324 | 9     | 500    |
	| 2425 | 9     | 500    |
	| 2425 | 10    | 200    |
	| 2425 | 11    | 200    |
	| 2425 | 12    | 200    |
	| 2425 | 12    | 500    |
	| 2425 | 12    | 500    |