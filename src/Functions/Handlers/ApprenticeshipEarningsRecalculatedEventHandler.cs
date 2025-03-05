using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Handlers
{
    public class ApprenticeshipEarningsRecalculatedEventHandler(ICommandHandler<RecalculateApprenticeshipPaymentsCommand> recalculateApprenticeshipPaymentsCommandHandler) : IHandleMessages<ApprenticeshipEarningsRecalculatedEvent>
    {
        public async Task Handle(ApprenticeshipEarningsRecalculatedEvent message, IMessageHandlerContext context)
        {
            await recalculateApprenticeshipPaymentsCommandHandler.Handle(
                new RecalculateApprenticeshipPaymentsCommand(message.ApprenticeshipKey,
                    message.DeliveryPeriods.ToEarnings(message.ApprenticeshipKey,
                        message.EarningsProfileId), message.StartDate, message.PlannedEndDate, message.AgeAtStartOfApprenticeship));
        }
    }
}
