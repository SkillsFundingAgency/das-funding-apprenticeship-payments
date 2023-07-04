using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, ApprenticeshipEntityModel apprenticeshipEntityModel);
}

public class FinalisedOnProgammeLearningPaymentEventBuilder : IFinalisedOnProgammeLearningPaymentEventBuilder
{
    public FinalisedOnProgammeLearningPaymentEvent Build(PaymentEntityModel payment, ApprenticeshipEntityModel apprenticeshipEntityModel)
    {
        return new FinalisedOnProgammeLearningPaymentEvent
        {
            CollectionYear = payment.CollectionYear,
            CollectionMonth = payment.CollectionPeriod,
            Amount = payment.Amount,
            ApprenticeshipKey = apprenticeshipEntityModel.ApprenticeshipKey,
            ApprenticeshipEarning = new ApprenticeshipEarning
            {
                Uln = apprenticeshipEntityModel.Uln,
                StartDate = apprenticeshipEntityModel.StartDate,
                PlannedEndDate = apprenticeshipEntityModel.PlannedEndDate,
                ProviderIdentifier = apprenticeshipEntityModel.Ukprn
            }
        };
    }
}