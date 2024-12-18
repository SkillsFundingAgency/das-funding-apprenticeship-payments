using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(Domain.Apprenticeship.Payment payment, Domain.Apprenticeship.IApprenticeship apprenticeship);
}

public class FinalisedOnProgammeLearningPaymentEventBuilder : IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(Domain.Apprenticeship.Payment payment, Domain.Apprenticeship.IApprenticeship apprenticeship)
    {
        var @event = new FinalisedOnProgammeLearningPaymentEvent();
        @event.Amount = payment.Amount;
        @event.Apprenticeship = new Apprenticeship();
        @event.Apprenticeship.StartDate = apprenticeship.StartDate;
        @event.Apprenticeship.ApprovalsApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId;
        @event.ApprenticeshipEmployerType = apprenticeship.EmployerType;

        @event.ApprenticeshipKey = apprenticeship.ApprenticeshipKey;
        @event.ApprenticeshipEarning = new ApprenticeshipEarning();
        @event.ApprenticeshipEarning.ApprenticeshipEarningsId = Guid.NewGuid();
        @event.ApprenticeshipEarning.DeliveryPeriod = payment.DeliveryPeriod;
        @event.ApprenticeshipEarning.DeliveryPeriodAmount = payment.Amount;
        @event.ApprenticeshipEarning.GovernmentContributionPercentage = CalculateGovernmentContributionPercentage(apprenticeship);
        @event.ApprenticeshipEarning.NumberOfInstalments = (short)apprenticeship.Payments.Count;
        @event.ApprenticeshipEarning.PlannedEndDate = apprenticeship.PlannedEndDate;
        @event.ApprenticeshipEarning.ProviderIdentifier = apprenticeship.Ukprn;
        @event.ApprenticeshipEarning.StartDate = apprenticeship.StartDate;
        @event.ApprenticeshipEarning.Uln = apprenticeship.Uln;
        @event.ApprenticeshipEarning.FundingLineType = payment.FundingLineType;
        @event.ApprenticeshipEarning.LearnerReference = apprenticeship.LearnerReference;

        @event.CollectionYear = payment.CollectionYear;
        @event.CollectionPeriod = payment.CollectionPeriod;
        @event.CourseCode = apprenticeship.CourseCode;

        @event.EmployerDetails = new EmployerDetails();
        @event.EmployerDetails.EmployingAccountId = apprenticeship.FundingEmployerAccountId;
        @event.EmployerDetails.FundingAccountId = apprenticeship.TransferSenderAccountId ?? apprenticeship.FundingEmployerAccountId;
        //@event.EmployerDetails.FundingCommitmentId = apprenticeship.ApprovalsApprenticeshipId;

        @event.FundingLineType = payment.FundingLineType;

        @event.EarningsProfileId = payment.EarningsProfileId;

        return @event;
    }

    private decimal CalculateGovernmentContributionPercentage(Domain.Apprenticeship.IApprenticeship apprenticeship)
    {
        if (apprenticeship.EmployerType == EmployerType.NonLevy && apprenticeship.AgeAtStartOfApprenticeship < 22)
            return 1;

        return 0.95m;
    }
}