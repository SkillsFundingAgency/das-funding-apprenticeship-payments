namespace SFA.DAS.Funding.ApprenticeshipPayments.Command;

public interface ICommandHandler<T>
{
    public Task Handle(T command);
}