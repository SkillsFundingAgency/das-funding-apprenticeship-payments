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

        public void CalculatePayments()
        {
            _payments.Clear();
            foreach (var earning in Earnings)
            {
                var collectionDate = DetermineCollectionDate(earning);
                var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, (short)collectionDate.Year, (byte)collectionDate.Month);
                _payments.Add(payment);
            }
        }

        public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth)
        {
            _earnings.Add(new Earning(academicYear, deliveryPeriod, amount, collectionYear, collectionMonth));
        }

        private static DateTime DetermineCollectionDate(Earning earning)
        {
            var censusDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            var collectionDate = new DateTime(earning.CollectionYear, earning.CollectionMonth, 1);
            if (collectionDate < censusDate)
            {
                collectionDate = censusDate;
            }

            return collectionDate;
        }
    }
}
