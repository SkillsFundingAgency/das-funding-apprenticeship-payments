using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;

internal static class PaymentExtensions
{
    internal static void AddPayment(this List<Payment> payments, Payment payment, DateTime now, AcademicYears academicYears)
    {
        // Check if the payment is within the current academic year or beyond
        if (payment.AcademicYear.ToDateTime(payment.DeliveryPeriod) >= academicYears.CurrentYear.StartDate)
        {
            payments.Add(payment);
        }

        // Check if the payment is within the previous academic year and before the hard close date
        if (now <= academicYears.PreviousYear.HardCloseDate && payment.AcademicYear == academicYears.PreviousYear.AcademicYear)
        {
            payments.Add(payment);
        }
    }
}
