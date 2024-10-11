namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public interface IRecalculateApprenticeshipPaymentsCommandHandler
{
    Task Recalculate(RecalculateApprenticeshipPaymentsCommand command);
}