using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments
{
    public class CalculateApprenticeshipPaymentsCommand
    {
        public CalculateApprenticeshipPaymentsCommand(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            EarningsGeneratedEvent = earningsGeneratedEvent;
        }

        public EarningsGeneratedEvent EarningsGeneratedEvent { get; }
    }
}
