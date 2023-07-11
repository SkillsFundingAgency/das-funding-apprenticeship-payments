namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public interface ICalculateRequiredLevyAmountCommandHandler
{
    Task Process(CalculateRequiredLevyAmountCommand command);
}