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
                var payment = new Payment(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.AcademicYear, earning.DeliveryPeriod);
                _payments.Add(payment);
            }
        }
    }
}
