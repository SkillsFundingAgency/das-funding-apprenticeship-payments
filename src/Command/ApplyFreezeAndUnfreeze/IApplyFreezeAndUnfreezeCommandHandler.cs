namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;

public interface IApplyFreezeAndUnfreezeCommandHandler
{
    Task Apply(ApplyFreezeAndUnfreezeCommand command);
}