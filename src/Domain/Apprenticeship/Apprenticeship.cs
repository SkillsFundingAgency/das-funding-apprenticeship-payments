using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.NServiceBus;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    [Table("Apprenticeship", Schema = "Domain")]
    public class Apprenticeship : AggregateRoot, IApprenticeship
    {
        private Apprenticeship() { }

        public Apprenticeship(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            ApprenticeshipKey = earningsGeneratedEvent.ApprenticeshipKey;
            _earnings = earningsGeneratedEvent.DeliveryPeriods.Select(y =>
            {
                var model = new Earning(earningsGeneratedEvent.ApprenticeshipKey, y.AcademicYear, y.Period, y.LearningAmount, y.CalenderYear, y.CalendarMonth, y.FundingLineType, earningsGeneratedEvent.EarningsProfileId);
                return model;
            }).ToList();
            EmployerType = earningsGeneratedEvent.EmployerType;
            StartDate = earningsGeneratedEvent.StartDate;
            Ukprn = earningsGeneratedEvent.ProviderId;
            Uln = long.Parse(earningsGeneratedEvent.Uln);
            PlannedEndDate = earningsGeneratedEvent.PlannedEndDate;
            CourseCode = earningsGeneratedEvent.TrainingCode;
            FundingEmployerAccountId = earningsGeneratedEvent.EmployerAccountId;
            ApprovalsApprenticeshipId = earningsGeneratedEvent.ApprovalsApprenticeshipId;
            TransferSenderAccountId = earningsGeneratedEvent.TransferSenderEmployerId;
            PaymentsFrozen = false;
            AgeAtStartOfApprenticeship = earningsGeneratedEvent.AgeAtStartOfApprenticeship;
            _payments = new List<Payment>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ApprenticeshipKey { get; }

        public long FundingEmployerAccountId { get; private set; }
        public EmployerType EmployerType { get; private set; }
        public long FundingCommitmentId { get; private set; }
        public long? TransferSenderAccountId { get; private set; }
        public long Uln { get; private set; }
        public long Ukprn { get; private set; }
        public DateTime PlannedEndDate { get; private set; }
        public string? CourseCode { get; private set; }
        public DateTime StartDate { get; private set; }
        public long ApprovalsApprenticeshipId { get; private set; }
        public bool PaymentsFrozen { get; private set; }
        public int AgeAtStartOfApprenticeship { get; private set; }

        private List<Earning> _earnings = new List<Earning>();
        public IReadOnlyCollection<Earning> Earnings => _earnings.AsReadOnly();
        private List<Payment> _payments = new List<Payment>();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        public void CalculatePayments(DateTime now)
        {
            _payments.Clear();
            foreach (var earning in Earnings)
            {
                var collectionPeriod = DetermineCollectionPeriod(earning, now);
                var payment = new Payment(ApprenticeshipKey, earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId);
                _payments.Add(payment);
            }
        }

        public void RecalculatePayments(DateTime now)
        {
            _payments.RemoveAll(p => !p.SentForPayment);

            var earningsToProcess = GetEarningsToProcess(now);

            foreach (var earning in earningsToProcess)
            {
                var collectionPeriod = DetermineCollectionPeriod(earning, now);

                if (!_payments.Any(p => p.DeliveryPeriod == earning.DeliveryPeriod && p.AcademicYear == earning.AcademicYear))
                {
                    var payment = new Payment(ApprenticeshipKey, earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId);
                    _payments.Add(payment);
                }
                else
                {
                    var existingPaidForDeliveryPeriod = _payments
                        .Where(p => p.DeliveryPeriod == earning.DeliveryPeriod && p.AcademicYear == earning.AcademicYear)
                        .Sum(p => p.Amount);

                    var payment = new Payment(ApprenticeshipKey, earning.AcademicYear, earning.DeliveryPeriod, earning.Amount - existingPaidForDeliveryPeriod, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId);
                    _payments.Add(payment);
                }
            }
        }

        public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth, string fundingLineType, Guid earningsProfileId)
        {
            _earnings.Add(new Earning(ApprenticeshipKey, academicYear, deliveryPeriod, amount, collectionYear, collectionMonth, fundingLineType, earningsProfileId));
        }

        public void ClearEarnings()
        {
            _earnings.Clear();
        }

        private static (short AcademicYear, byte Period) DetermineCollectionPeriod(Earning earning, DateTime now)
        {
            var censusDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local).AddMonths(1).AddDays(-1);
            var collectionDate = new DateTime(earning.CollectionYear, earning.CollectionMonth, 1, 0, 0, 0, DateTimeKind.Local);
            if (collectionDate >= censusDate)
            {
                return (earning.AcademicYear, earning.DeliveryPeriod);
            }

            collectionDate = censusDate;
            return CollectionDateToPeriod(collectionDate);
        }

        private static (short AcademicYear, byte Period) CollectionDateToPeriod(DateTime collectionDate)
        {
            var period = collectionDate.Month - 7;
            if (period <= 0)
            {
                period = 12 + period;
            }

            short academicYear;
            var year = short.Parse($"{collectionDate.Year}".Substring(2, 2));
            if (collectionDate.Month < 8)
                academicYear = short.Parse($"{year - 1}{year}");
            else
                academicYear = short.Parse($"{year}{year + 1}");

            return (academicYear, (byte)period);
        }

        // When new earnings are generated they may not cover a period that has already been paid for
        // For these periods earnings of zero for that month need to be generated for calculation purposes
        private  List<Earning> GetEarningsToProcess(DateTime now)
        {
            // Put earnings into a new list, this is for processing only as we will be adding zero earnings
            var earningsToProcess = Earnings.ToList();

            // Cycle through payments and add zero earnings for any periods that have had payments made before recalc
            foreach(Payment payment in _payments)
			{
				if (!earningsToProcess.Any(e=> e.DeliveryPeriod == payment.DeliveryPeriod && e.AcademicYear == payment.AcademicYear))
                {
                    var earning = new Earning(ApprenticeshipKey, payment.AcademicYear, payment.DeliveryPeriod, 0, (short)now.Year, (byte)now.Month, payment.FundingLineType, payment.EarningsProfileId);
                    earningsToProcess.Add(earning);
                }
            }

            // Sorting the list has no technical purpose, it is just to make manual validation easier
            return earningsToProcess
                .OrderBy(x => x.AcademicYear)
                .ThenBy(x => x.DeliveryPeriod)
                .ToList();
        }

        public void MarkPaymentsAsFrozen(short collectionYear, byte collectionPeriod)
        {
            foreach (var payment in DuePayments(collectionYear, collectionPeriod))
            {
                payment.MarkAsNotPaid();
            }
        }

        public ReadOnlyCollection<Payment> DuePayments(short collectionYear, byte collectionPeriod)
        {
            return _payments.Where(x => x.CollectionPeriod == collectionPeriod && x.CollectionYear == collectionYear && !x.SentForPayment).ToList().AsReadOnly();
        }

        public void UnfreezeFrozenPayments(short collectionYear, byte collectionPeriod, short currentAcademicYear, short previousAcademicYear, DateTime previousAcademicYearHardClose, DateTime currentDate)
        {
            var validAcademicYears = new List<short> { currentAcademicYear };

            if (previousAcademicYearHardClose.Date >= currentDate.Date)
            {
                validAcademicYears.Add(previousAcademicYear);
            }

            foreach (var payment in _payments.Where(x => x.NotPaidDueToFreeze && validAcademicYears.Contains(x.CollectionYear)))
            {
                payment.Unfreeze(collectionYear, collectionPeriod);
            }
        }

        public void MarkPaymentsAsSent(short collectionYear, byte collectionPeriod)
        {
            foreach (var payment in DuePayments(collectionYear, collectionPeriod))
            {
                payment.Send();
            }
        }

        public void FreezePayments()
        {
            PaymentsFrozen = true;
        }

        public void UnfreezePayments()
        {
            PaymentsFrozen = false;
        }
    }
}