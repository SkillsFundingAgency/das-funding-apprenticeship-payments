using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount
{
    internal static class CalculateRequiredLevyAmountCommandExtensions
    {
        internal static CalculatedRequiredLevyAmount MapToCalculatedRequiredLevyAmountEvent(this CalculateRequiredLevyAmountCommand command)
        {
            var e = new CalculatedRequiredLevyAmount();
            e.AccountId = command.Data.EmployerDetails.EmployingAccountId;
            e.ActualEndDate = null;
            e.AgreedOnDate = null;
            e.AgreementId = null;
            e.AmountDue = command.Data.Amount;
            e.ApprenticeshipEmployerType = 0;
            e.ApprenticeshipId = command.Data.EmployerDetails.FundingCommitmentId;
            e.ApprenticeshipPriceEpisodeId = null;
            e.ClawbackSourcePaymentEventId = null;
            e.CollectionPeriod = new();
            e.CollectionPeriod.AcademicYear = command.Data.CollectionYear;
            // e.CollectionPeriod.Period  =  command.Data.Period // TODO: ReleasePaymentsCommand -> CollectionPeriod
            e.CompletionStatus = 1; // ongoing
            e.CompletionAmount = 0;
            e.ContractType = ContractType.Act1;
            e.DeliveryPeriod = command.Data.ApprenticeshipEarnings.DeliveryPeriod;
            e.EarningEventId = command.Data.ApprenticeshipEarnings.ApprenticeshipEarningsId;
            e.EventId = Guid.NewGuid();
            e.EventTime = DateTime.UtcNow;
            e.IlrFileName = null;
            //e.IlrSubmissionDateTime = null; // TODO: Modify IPaymentsEvent in PV2
            e.InstalmentAmount = command.Data.ApprenticeshipEarnings.DeliveryPeriodAmount;
            e.JobId = 0; // TODO: tbc, make it configurable?
            e.Learner = new();
            e.Learner.ReferenceNumber = null;
            e.Learner.Uln = command.Data.ApprenticeshipEarnings.Uln;
            e.LearningAim = new();
            e.LearningAim.FrameworkCode = 0;
            e.LearningAim.FundingLineType = command.Data.ApprenticeshipEarnings.FundingLineType;
            e.LearningAim.PathwayCode = 0;
            e.LearningAim.ProgrammeType = 0;
            e.LearningAim.Reference = "ZPROG001";
            e.LearningAim.SequenceNumber = 0;
            e.LearningAim.StandardCode = command.Data.CourseCode;
            e.LearningAim.StartDate = command.Data.Apprenticeship.StartDate;
            e.LearningAimSequenceNumber = 0;
            e.LearningStartDate = command.Data.Apprenticeship.StartDate;
            e.NumberOfInstalments = command.Data.ApprenticeshipEarnings.NumberOfInstalments;
            e.OnProgrammeEarningType = OnProgrammeEarningType.Learning;
            e.PlannedEndDate = command.Data.ApprenticeshipEarnings.PlannedEndDate;
            e.PriceEpisodeIdentifier = "";
            e.Priority = 0;
            e.ReportingAimFundingLineType = "";
            e.SfaContributionPercentage = command.Data.ApprenticeshipEarnings.GovernmentContributionPercentage;
            e.StartDate = command.Data.Apprenticeship.StartDate;
            e.TransferSenderAccountId = command.Data.EmployerDetails.FundingAccountId;
            e.Ukprn = command.Data.ApprenticeshipEarnings.ProviderIdentifier;
            e.TransactionType = TransactionType.Learning;

            return e;
        }
    }
}
