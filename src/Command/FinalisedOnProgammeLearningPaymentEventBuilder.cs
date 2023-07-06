using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, ApprenticeshipEntityModel apprenticeship);
}

public class FinalisedOnProgammeLearningPaymentEventBuilder : IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, ApprenticeshipEntityModel apprenticeship)
    {
        var @event = new FinalisedOnProgammeLearningPaymentEvent();
        @event.Amount = payment.Amount;
        @event.Apprenticeship = new Apprenticeship();
        @event.Apprenticeship.StartDate = apprenticeship.StartDate;

        @event.ApprenticeshipKey = apprenticeship.ApprenticeshipKey;
        @event.ApprenticeshipEarnings = new ApprenticeshipEarning();
        @event.ApprenticeshipEarnings.ApprenticeshipEarningsId = Guid.NewGuid();
        @event.ApprenticeshipEarnings.DeliveryPeriod = payment.DeliveryPeriod;
        @event.ApprenticeshipEarnings.DeliveryPeriodAmount = payment.Amount;
        @event.ApprenticeshipEarnings.GovernmentContributionPercentage = 0.95m;
        @event.ApprenticeshipEarnings.NumberOfInstalments = (short)apprenticeship.Payments.Count;

        @event.CollectionYear = payment.CollectionYear;
        @event.CollectionPeriod = payment.CollectionPeriod;
        @event.CourseCode = apprenticeship.CourseCode;

        @event.EmployerDetails = new EmployerDetails();
        @event.EmployerDetails.EmployingAccountId = apprenticeship.FundingEmployerAccountId;
        @event.EmployerDetails.FundingAccountId = apprenticeship.TransferSenderAccountId ?? apprenticeship.FundingEmployerAccountId;
        @event.EmployerDetails.FundingCommitmentId = apprenticeship.FundingCommitmentId;

        return @event;
    }
}