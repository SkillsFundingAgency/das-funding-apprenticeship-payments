using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

[Table("Apprenticeship", Schema = "Domain")]
public class Apprenticeship : AggregateRoot, IApprenticeship
{
    private Apprenticeship() { }

    public Apprenticeship(EarningsGeneratedEvent earningsGeneratedEvent)
    {
        ApprenticeshipKey = earningsGeneratedEvent.ApprenticeshipKey;

        _earnings = earningsGeneratedEvent.DeliveryPeriods.Select(dp =>
        {
            var model = new Earning(
                earningsGeneratedEvent.ApprenticeshipKey, 
                dp.AcademicYear, 
                dp.Period, 
                dp.LearningAmount, 
                dp.CalenderYear, 
                dp.CalendarMonth, 
                dp.FundingLineType, 
                earningsGeneratedEvent.EarningsProfileId,
                dp.InstalmentType);
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
    public long? TransferSenderAccountId { get; private set; }
    public long Uln { get; private set; }
    public long Ukprn { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public string? CourseCode { get; private set; }
    public DateTime StartDate { get; private set; }
    public long ApprovalsApprenticeshipId { get; private set; }
    public bool PaymentsFrozen { get; private set; }
    public int AgeAtStartOfApprenticeship { get; private set; }
    public string? LearnerReference { get; private set; }

    private List<Earning> _earnings = new List<Earning>();
    public IReadOnlyCollection<Earning> Earnings => _earnings.AsReadOnly();
    private List<Payment> _payments = new List<Payment>();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public void CalculatePayments(DateTime now)
    {
        _payments.Clear();
        foreach (var earning in Earnings)
        {
            var collectionPeriod = DetermineCollectionPeriod(earning);
            var payment = new Payment(ApprenticeshipKey, earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId, earning.InstalmentType);
            _payments.Add(payment);
        }
    }

    public void RecalculatePayments(DateTime now)
    {
        _payments.RemoveAll(p => !p.SentForPayment);

        var earningsToProcess = GetEarningsToProcess(now);

        foreach (var earning in earningsToProcess)
        {
            var collectionPeriod = DetermineCollectionPeriod(earning);

            if (!_payments.Any(p => p.DeliveryPeriod == earning.DeliveryPeriod && p.AcademicYear == earning.AcademicYear && p.PaymentType.ToInstalmentType() == earning.InstalmentType))
            {
                var payment = new Payment(ApprenticeshipKey, earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId, earning.InstalmentType);
                _payments.Add(payment);
            }
            else
            {
                var existingPaidForDeliveryPeriod = _payments
                    .Where(p => p.DeliveryPeriod == earning.DeliveryPeriod && p.AcademicYear == earning.AcademicYear && p.PaymentType.ToInstalmentType() == earning.InstalmentType)
                    .Sum(p => p.Amount);

                if (earning.Amount - existingPaidForDeliveryPeriod == 0) continue;

                var payment = new Payment(ApprenticeshipKey, earning.AcademicYear, earning.DeliveryPeriod, earning.Amount - existingPaidForDeliveryPeriod, collectionPeriod.AcademicYear, collectionPeriod.Period, earning.FundingLineType, earning.EarningsProfileId, earning.InstalmentType);
                _payments.Add(payment);
            }
        }

    }

    public void AddEarning(short academicYear, byte deliveryPeriod, decimal amount, short collectionYear, byte collectionMonth, string fundingLineType, Guid earningsProfileId, string instalmentType)
    {
        _earnings.Add(new Earning(ApprenticeshipKey, academicYear, deliveryPeriod, amount, collectionYear, collectionMonth, fundingLineType, earningsProfileId, instalmentType));
    }

    public void ClearEarnings()
    {
        _earnings.Clear();
    }

    private static (short AcademicYear, byte Period) DetermineCollectionPeriod(Earning earning)
    {
        return (earning.AcademicYear, earning.DeliveryPeriod);
    }

    // When new earnings are generated they may not cover a period that has already been paid for
    // For these periods earnings of zero for that month need to be generated for calculation purposes
    private List<Earning> GetEarningsToProcess(DateTime now)
    {
        // Put earnings into a new list, this is for processing only as we will be adding zero earnings
        var earningsToProcess = Earnings.ToList();

        // Cycle through payments and add zero earnings for any periods that have had payments made before recalc
        foreach (Payment payment in _payments)
        {
            if (!earningsToProcess.Any(e => e.DeliveryPeriod == payment.DeliveryPeriod && e.AcademicYear == payment.AcademicYear && e.InstalmentType == payment.PaymentType.ToInstalmentType()))
            {
                var earning = new Earning(ApprenticeshipKey, payment.AcademicYear, payment.DeliveryPeriod, 0, (short)now.Year, (byte)now.Month, payment.FundingLineType, payment.EarningsProfileId, payment.PaymentType);
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
        return _payments.Where(x => x.CollectionPeriod <= collectionPeriod && x.CollectionYear == collectionYear && !x.SentForPayment && !x.NotPaidDueToFreeze).ToList().AsReadOnly();
    }

    public void UnfreezeFrozenPayments(short currentAcademicYear, short previousAcademicYear, DateTime previousAcademicYearHardClose, DateTime currentDate)
    {
        var validAcademicYears = new List<short> { currentAcademicYear };

        if (previousAcademicYearHardClose.Date >= currentDate.Date)
        {
            validAcademicYears.Add(previousAcademicYear);
        }

        foreach (var payment in _payments.Where(x => x.NotPaidDueToFreeze && validAcademicYears.Contains(x.CollectionYear)))
        {
            payment.Unfreeze();
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

    public void SetLearnerReference(string learnerReference)
    {
        LearnerReference = learnerReference;
    }

    public Payment SendPayment(Guid paymentKey, short collectionYear, byte collectionPeriod)
    {
        var payment = Payments.Single(x => x.Key == paymentKey);
        payment.MarkAsSent(collectionYear, collectionPeriod);
        return payment;
    }

    public void Update(DateTime startDate, DateTime plannedEndDate, int ageAtStartOfApprenticeship)
    {
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        AgeAtStartOfApprenticeship = ageAtStartOfApprenticeship;
    }
}