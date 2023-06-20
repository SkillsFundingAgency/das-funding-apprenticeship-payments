#region Assembly SFA.DAS.Funding.ApprenticeshipPayments.Types, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Users\rusla\.nuget\packages\sfa.das.funding.apprenticeshippayments.types\0.1.8-prerelease-27\lib\net6.0\SFA.DAS.Funding.ApprenticeshipPayments.Types.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System;
using NServiceBus;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredLevyAmount : IPaymentsEvent, IJobMessage, IPaymentsMessage, SFA.DAS.Payments.Messages.Core.Events.IEvent, NServiceBus.IEvent, IMessage
    {
        public string EarningSource => "SubmitLearnerDataFundingPlatform";

        public int Priority { get; set; }

        public string? AgreementId { get; set; }

        public DateTime? AgreedOnDate { get; set; }

        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }

        public TransactionType TransactionType { get; set; }

        public Guid EarningEventId { get; set; }

        public Guid? ClawbackSourcePaymentEventId { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public decimal AmountDue { get; set; }

        public byte DeliveryPeriod { get; set; }

        public long? AccountId { get; set; }

        public long? TransferSenderAccountId { get; set; }

        public ContractType ContractType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? PlannedEndDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public byte CompletionStatus { get; set; }

        public decimal CompletionAmount { get; set; }

        public decimal InstalmentAmount { get; set; }

        public short NumberOfInstalments { get; set; }

        public DateTime? LearningStartDate { get; set; }

        public long? ApprenticeshipId { get; set; }

        public long? ApprenticeshipPriceEpisodeId { get; set; }

        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }

        public string ReportingAimFundingLineType { get; set; }

        public long? LearningAimSequenceNumber { get; set; }

        public long JobId { get; set; }

        public DateTimeOffset EventTime { get; set; }

        public Guid EventId { get; set; }

        public long Ukprn { get; set; }

        public Learner Learner { get; set; } = new Learner();


        public LearningAim LearningAim { get; set; } = new LearningAim();


        public DateTime IlrSubmissionDateTime { get; set; }

        public string IlrFileName { get; set; }

        public CollectionPeriod CollectionPeriod { get; set; } = new CollectionPeriod();

    }
}
