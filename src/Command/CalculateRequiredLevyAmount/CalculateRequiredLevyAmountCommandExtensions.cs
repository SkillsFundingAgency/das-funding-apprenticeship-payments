using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount
{
    internal static class CalculateRequiredLevyAmountCommandExtensions
    {
        internal static CalculateOnProgrammePayment MapToCalculateOnProgrammePaymentEvent(this CalculateRequiredLevyAmountCommand command)
        {
            var e = new CalculateOnProgrammePayment();
            ArgumentNullException.ThrowIfNull(command.Data);
            ArgumentNullException.ThrowIfNull(command.Data.EmployerDetails);
            ArgumentNullException.ThrowIfNull(command.Data.ApprenticeshipEmployerType);
            ArgumentNullException.ThrowIfNull(command.Data.Apprenticeship);
            ArgumentNullException.ThrowIfNull(command.Data.ApprenticeshipEarning);

            e.AccountId = command.Data.EmployerDetails.EmployingAccountId;
            e.ActualEndDate = null;
            e.AgreedOnDate = null;
            e.AmountDue = command.Data.Amount;
            e.ApprenticeshipEmployerType = command.Data.ApprenticeshipEmployerType == EmployerType.Levy ? ApprenticeshipEmployerType.Levy : ApprenticeshipEmployerType.NonLevy;
            e.ApprenticeshipId = command.Data.Apprenticeship.ApprovalsApprenticeshipId;
            e.ApprenticeshipPriceEpisodeId = null;
            e.CollectionPeriod = new();
            e.CollectionPeriod.AcademicYear = command.Data.CollectionYear;
            e.CollectionPeriod.Period = command.Data.CollectionPeriod;
            e.CompletionStatus = 1; // ongoing
            e.CompletionAmount = 0;
            e.DeliveryPeriod = command.Data.ApprenticeshipEarning.DeliveryPeriod;
            e.EventId = Guid.NewGuid();
            e.EventTime = DateTime.UtcNow;
            e.InstalmentAmount = command.Data.ApprenticeshipEarning.DeliveryPeriodAmount;
            e.Learner = new();
            e.Learner.ReferenceNumber = command.Data.ApprenticeshipEarning.Uln.ToString();
            e.Learner.Uln = command.Data.ApprenticeshipEarning.Uln;
            e.LearningAim = new();
            e.LearningAim.FrameworkCode = 0;
            e.LearningAim.FundingLineType = command.Data.ApprenticeshipEarning.FundingLineType;
            e.LearningAim.PathwayCode = 0;
            e.LearningAim.ProgrammeType = 25;
            e.LearningAim.Reference = "ZPROG001";
            e.LearningAim.SequenceNumber = 0;
            e.LearningAim.StandardCode = Convert.ToInt32(command.Data.CourseCode);
            e.LearningAim.StartDate = command.Data.Apprenticeship.StartDate;
            e.LearningStartDate = command.Data.Apprenticeship.StartDate;
            e.NumberOfInstalments = command.Data.ApprenticeshipEarning.NumberOfInstalments;
            e.OnProgrammeEarningType = OnProgrammeEarningType.Learning;
            e.PlannedEndDate = command.Data.ApprenticeshipEarning.PlannedEndDate!.Value;
            e.PriceEpisodeIdentifier = "";
            e.SfaContributionPercentage = command.Data.ApprenticeshipEarning.GovernmentContributionPercentage;
            e.StartDate = command.Data.Apprenticeship.StartDate;
            e.TransferSenderAccountId = command.Data.EmployerDetails.FundingAccountId;
            e.Ukprn = command.Data.ApprenticeshipEarning.ProviderIdentifier;
            e.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService; 
            return e;
        }
    }
}
