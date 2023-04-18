using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments
{
    public class CalculateApprenticeshipPaymentsCommandHandler : ICalculateApprenticeshipPaymentsCommandHandler
    {
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IMessageSession _messageSession;
        private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;

        public CalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipFactory apprenticeshipFactory, IMessageSession messageSession, IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder)
        {
            _apprenticeshipFactory = apprenticeshipFactory;
            _messageSession = messageSession;
            _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
        }

        public async Task<Apprenticeship> Calculate(CalculateApprenticeshipPaymentsCommand command)
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew(command.ApprenticeshipEntity);
            apprenticeship.CalculatePayments(DateTime.Now);
            await _messageSession.Publish(_paymentsGeneratedEventBuilder.Build(apprenticeship));
            return apprenticeship;
        }
    }
}
