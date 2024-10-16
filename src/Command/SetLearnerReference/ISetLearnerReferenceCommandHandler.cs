namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;

public interface ISetLearnerReferenceCommandHandler
{
    Task Set(SetLearnerReferenceCommand command);
}