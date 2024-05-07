using SFA.DAS.Payments.Model.Core.OnProgramme;
using System.Collections.ObjectModel;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship
{
    public class Apprenticeship : AggregateRoot
    {
        public Apprenticeship(Guid apprenticeshipKey)
        {
            ApprenticeshipKey = apprenticeshipKey;
            _earnings = new List<Earning>();
            _payments = new List<Payment>();
        }

        public Apprenticeship(Guid apprenticeshipKey, List<Earning> earnings, List<Payment> payments)
        {
            ApprenticeshipKey = apprenticeshipKey;
            _earnings = earnings;
            _payments = payments;
        }

        public Guid ApprenticeshipKey { get; }

        private readonly List<Earning> _earnings;
        public ReadOnlyCollection<Earning> Earnings => _earnings.AsReadOnly();
        private readonly List<Payment> _payments;
        public ReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        public void CalculatePayments(DateTime now)
        {
            _payments.Clear();
            foreach (var earning in Earnings)
            {
                var collectionPeriod = DetermineCollectionPeriod(earning, now);
                var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId);
                _payments.Add(payment);
            }
        }

        public void RecalculatePayments(DateTime now)
        {
            _payments.RemoveAll(p => !p.SentForPayment);

            var earningsToProcess = GetEarningsToProcess();

            foreach (var earning in earningsToProcess)
            {
                var collectionPeriod = DetermineCollectionPeriod(earning, now);

                if (!_payments.Any(p => p.DeliveryPeriod == earning.DeliveryPeriod && p.AcademicYear == earning.AcademicYear))
                {
                    var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId);
                    _payments.Add(payment);
                }
                else
                {
                    var existingPaidForDeliveryPeriod = _payments
                        .Where(p => p.DeliveryPeriod == earning.DeliveryPeriod && p.AcademicYear == earning.AcademicYear)
                        .Sum(p => p.Amount);

                    var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount - existingPaidForDeliveryPeriod, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId);
                    _payments.Add(payment);
                }
            }
        }

        public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth, string fundingLineType, Guid earningsProfileId)
        {
            _earnings.Add(new Earning(academicYear, deliveryPeriod, amount, collectionYear, collectionMonth, fundingLineType, earningsProfileId));
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
        private  List<Earning> GetEarningsToProcess()
        {
            var earningsToProcess = Earnings.ToList();

            foreach(Payment payment in _payments)
			{
				if (!earningsToProcess.Any(e=> e.DeliveryPeriod == payment.DeliveryPeriod && e.AcademicYear == payment.AcademicYear))
                {
					int collectionMonth = payment.DeliveryPeriod + 7;
					if (collectionMonth > 12)
					{
						collectionMonth -= 12;
					}

					var earning = new Earning(payment.AcademicYear, payment.DeliveryPeriod, 0, payment.CollectionYear, (byte)collectionMonth, payment.FundingLineType, payment.EarningsProfileId);
					earningsToProcess.Add(earning);
				}
			}

            return earningsToProcess;
        }
    }
}