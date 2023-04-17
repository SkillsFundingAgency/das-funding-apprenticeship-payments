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

        public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount)
        {
            _earnings.Add(new Earning(academicYear, deliveryPeriod, amount));
        }

        public void CalculatePayments()
        {
            _payments.Clear();
            foreach (var earning in Earnings)
            {
                var paymentPeriod = GetPaymentPeriod(earning.AcademicYear, earning.DeliveryPeriod);
                var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, paymentPeriod.AcademicYear, paymentPeriod.Period);
                _payments.Add(payment);
            }
        }

        private (short AcademicYear, byte Period) GetPaymentPeriod(short earningAcademicYear, byte deliveryPeriod)
        {
            if (deliveryPeriod < 12)
                return (earningAcademicYear, (byte)(deliveryPeriod + 1));

            var lastTwo = short.Parse($"{earningAcademicYear}".Substring(2, 2));

            return (short.Parse($"{lastTwo}{lastTwo + 1}"), 1);
        }
    }
}
