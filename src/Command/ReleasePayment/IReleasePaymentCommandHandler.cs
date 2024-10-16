namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;

public interface IReleasePaymentCommandHandler
{
    Task Release(ReleasePaymentCommand command);
}