﻿CREATE TABLE [Domain].[Apprenticeship]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[FundingEmployerAccountId] BIGINT NOT NULL,
	[EmployerType] NVARCHAR(50) NOT NULL,
	[FundingCommitmentId] BIGINT NOT NULL,
	[TransferSenderAccountId] BIGINT NULL,
	[Uln] BIGINT NOT NULL,
	[Ukprn] BIGINT NOT NULL,
	[PlannedEndDate] DATETIME NOT NULL,
	[CourseCode] NVARCHAR(10) NULL,
	[StartDate] DATETIME NOT NULL,
	[ApprovalsApprenticeshipId] BIGINT NOT NULL,
	[PaymentsFrozen] BIT NOT NULL DEFAULT(0)
)
