using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Types;

public class CalculatedRequiredLevyAmountEvent
{
    public long AccountId { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public DateTime? AgreedOnDate { get; set; }
    public string? AgreementId { get; set; }
    public decimal AmountDue { get; set; }
    public byte ApprenticeshipEmployerType { get; set; }
    public long ApprenticeshipId { get; set; }
    public string ApprenticeshipPriceEpisodeId { get; set; }
    public Guid? ClawbackSourcePaymentEventId { get; set; }
    public CollectionPeriod CollectionPeriod { get; set; } = new();
    public byte CompletionStatus { get; set; }
    public string ContractType { get; set; }
    public string DeliveryPeriod { get; set; }
    public Guid EarningEventId { get; set; }
    public Guid EventId { get; set; }
    public DateTime EventTime { get; set; }
    public string? IlrFileName { get; set; }
    public DateTime? IlrSubmissionDateTime { get; set; }
    public decimal InstalmentAmount { get; set; }
    public string JobId { get; set; } // TODO: TBC
    public Learner Learner { get; set; } = new();
    public LearningAim LearningAim { get; set; } = new();
    public int LearningAimSequenceNumber { get; set; }
    public DateTime LearningStartDate { get; set; }
    public int NumberOfInstalments { get; set; }
    public OnProgrammeEarningType OnProgrammeEarningType { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public string PriceEpisodeIdentifier { get; set; }
    public int Priority { get; set; }
    public string ReportingAimFundingLineType { get; set; }
    public decimal SfaContributionPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public TransactionType TransactionType { get; set; }
    public long TransferSenderAccountId { get; set; }
    public long Ukprn { get; set; }
    public string EarningSource { get; set; } = "SubmitLearnerDataFundingPlatform";
}