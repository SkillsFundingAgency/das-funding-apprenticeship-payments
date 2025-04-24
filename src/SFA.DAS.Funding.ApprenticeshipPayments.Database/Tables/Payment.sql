CREATE TABLE [Domain].[Payment]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL,
	[EarningsProfileId] UNIQUEIDENTIFIER NOT NULL,
	[AcademicYear] SMALLINT NOT NULL,
	[DeliveryPeriod] TINYINT NOT NULL,
	[Amount] DECIMAL(15, 5) NOT NULL,
	[CollectionYear] SMALLINT NOT NULL,
	[CollectionPeriod] TINYINT NOT NULL,
	[SentForPayment] BIT NOT NULL DEFAULT(0),
	[FundingLineType] NVARCHAR(50) NOT NULL,
	[NotPaidDueToFreeze] BIT NOT NULL DEFAULT(0), 
	[PaymentType] NVARCHAR(20) NULL
)
GO
CREATE INDEX IX_Payment_ApprenticeshipKey ON [Domain].[Payment] (ApprenticeshipKey)
GO
CREATE INDEX IX_Payment_Collection ON [Domain].[Payment] (CollectionYear, CollectionPeriod)
GO