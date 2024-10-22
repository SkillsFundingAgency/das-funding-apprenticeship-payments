using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using System.Collections.ObjectModel;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public interface IApprenticeship : IAggregateRoot
    {
        public Guid ApprenticeshipKey { get; }
        public long FundingEmployerAccountId { get; }
        public EmployerType EmployerType { get; }
        public long FundingCommitmentId { get; }
        public long? TransferSenderAccountId { get; }
        public long Uln { get; }
        public long Ukprn { get; }
        public DateTime PlannedEndDate { get; }
        public string? CourseCode { get; }
        public DateTime StartDate { get; }
        public long ApprovalsApprenticeshipId { get; }
        public bool PaymentsFrozen { get; }
        public int AgeAtStartOfApprenticeship { get; }
        public string? LearnerReference { get; }
        public IReadOnlyCollection<Earning> Earnings { get; }
        public IReadOnlyCollection<Payment> Payments { get; }

        public void CalculatePayments(DateTime now);
        public void RecalculatePayments(DateTime now);
        public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth, string fundingLineType, Guid earningsProfileId);
        public void ClearEarnings();
        public void MarkPaymentsAsFrozen(short collectionYear, byte collectionPeriod);
        public ReadOnlyCollection<Payment> DuePayments(short collectionYear, byte collectionPeriod);
        public void UnfreezeFrozenPayments(short collectionYear, byte collectionPeriod, short currentAcademicYear, short previousAcademicYear, DateTime previousAcademicYearHardClose, DateTime currentDate);
        public void MarkPaymentsAsSent(short collectionYear, byte collectionPeriod);
        public void FreezePayments();
        public void UnfreezePayments();
        public void SetLearnerReference(string learnerReference);
        public void SendPayment(Guid paymentKey, Func<Payment, IApprenticeship, IDomainEvent> eventBuilder);
    }
}