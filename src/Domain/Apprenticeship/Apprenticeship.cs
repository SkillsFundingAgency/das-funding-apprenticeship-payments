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
                var collectionPeriod = DeterminePaymentPeriod(earning, now);
                var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, (short)collectionPeriod.AcademicYear, (byte)collectionPeriod.Period);
                _payments.Add(payment);
            }
        }

        public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth)
        {
            _earnings.Add(new Earning(academicYear, deliveryPeriod, amount, collectionYear, collectionMonth));
        }

        private (short AcademicYear, byte Period) DeterminePaymentPeriod(Earning earning, DateTime now)
        {
            var censusDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
            var collectionDate = new DateTime(earning.CollectionYear, earning.CollectionMonth, 1);
            if (collectionDate >= censusDate)
            {
                return (earning.AcademicYear, earning.DeliveryPeriod);
            }

            collectionDate = censusDate;
            return CollectionDateToPeriod(collectionDate);
        }

        private (short AcademicYear, byte Period) CollectionDateToPeriod(DateTime collectionDate)
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
    }
}