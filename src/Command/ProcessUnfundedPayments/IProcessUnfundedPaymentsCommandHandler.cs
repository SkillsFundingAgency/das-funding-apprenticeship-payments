namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;

public interface IProcessUnfundedPaymentsCommandHandler
{
    Task Process(ProcessUnfundedPaymentsCommand command);
}