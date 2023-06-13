using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount
{
    internal static class CalculateRequiredLevyAmountCommandExtensions
    {
        internal static CalculatedRequiredLevyAmountEvent MapToCalculatedRequiredLevyAmountEvent(this CalculateRequiredLevyAmountCommand command)
        {
            var e = new CalculatedRequiredLevyAmountEvent();
            e.AccountId = command.Data.EmployerDetails.EmployingAccountId;
            e.ActualEndDate = command.Data.ActualEndDate;
            e.AgreedOnDate = null;
            e.AgreementId = null;
            e.AmountDue = command.Data.Amount;
            e.ApprenticeshipEmployerType = 0;
            e.ApprenticeshipId = command.Data.EmployerDetails.FundingCommitmentId;
            e.ApprenticeshipPriceEpisodeId = command.Data.ApprenticeshipEarnings.FundingPeriodId;
            e.ClawbackSourcePaymentEventId = null;
            e.CollectionPeriod.AcademicYear = command.Data.CollectionYear;
            // e.CollectionPeriod.Period  =  command.Data.Period // TODO: ReleasePaymentsCommand -> CollectionPeriod
            e.CompletionStatus = 1;
            e.ContractType = "ACT1";
            e.DeliveryPeriod = command.Data.ApprenticeshipEarnings.DeliveryPeriod;
            e.EarningEventId = command.Data.ApprenticeshipEarnings.ApprenticeshipEarningsId;
            e.EventId = Guid.NewGuid();
            e.EventTime = DateTime.UtcNow;
            e.IlrFileName = null;
            e.IlrSubmissionDateTime = null;
            e.InstalmentAmount = command.Data.ApprenticeshipEarnings.DeliveryPeriodAmount;
            e.JobId = "TBC";
            e.Learner.ReferenceNumber = null;
            e.Learner.Uln = command.Data.ApprenticeshipEarnings.Uln;
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
            e.TransactionType = TransactionType.Learning;
            e.TransferSenderAccountId = command.Data.EmployerDetails.FundingAccountId;
            e.Ukprn = command.Data.ApprenticeshipEarnings.ProviderIdentifier;
            e.EarningSource = "SubmitLearnerDataFundingPlatform";

            return e;
        }
    }
}
