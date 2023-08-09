namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;

public interface ICalculateRequiredLevyAmountCommandHandler
{
    Task Send(CalculateRequiredLevyAmountCommand command);
    Task Publish(CalculateRequiredLevyAmountCommand command);
}