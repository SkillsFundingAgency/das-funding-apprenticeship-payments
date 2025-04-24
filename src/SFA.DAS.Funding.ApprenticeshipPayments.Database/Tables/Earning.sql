CREATE TABLE [Domain].[Earning]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL,
	[EarningsProfileId] UNIQUEIDENTIFIER NOT NULL,
	[DeliveryPeriod] TINYINT NOT NULL,
	[AcademicYear] SMALLINT NOT NULL,
	[CollectionMonth] TINYINT NOT NULL,
	[CollectionYear] SMALLINT NOT NULL,
	[Amount] DECIMAL(15, 5) NOT NULL,
	[FundingLineType] NVARCHAR(50) NOT NULL, 
	[InstalmentType] NVARCHAR(20) NULL
)
GO
CREATE INDEX IX_Earning_ApprenticeshipKey ON [Domain].[Earning] (ApprenticeshipKey)
GO
