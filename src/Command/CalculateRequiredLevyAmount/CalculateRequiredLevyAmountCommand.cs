using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public class CalculateRequiredLevyAmountCommand
{
    public FinalisedOnProgammeLearningPaymentEvent Data { get; }

    public CalculateRequiredLevyAmountCommand(FinalisedOnProgammeLearningPaymentEvent data)
    {
        Data = data;
    }
}