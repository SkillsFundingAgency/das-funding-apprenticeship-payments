namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public interface ICalculateRequiredLevyAmountCommandHandler
{
    Task Publish(CalculateRequiredLevyAmountCommand command);
}