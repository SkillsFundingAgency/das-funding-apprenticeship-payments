using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;

public class CalculateApprenticeshipPaymentsCommandHandler : ICalculateApprenticeshipPaymentsCommandHandler
{
    private readonly IApprenticeshipFactory _apprenticeshipFactory;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;
    private readonly ISystemClockService _systemClockService;

    public CalculateApprenticeshipPaymentsCommandHandler(
        IApprenticeshipFactory apprenticeshipFactory,
        IDasServiceBusEndpoint busEndpoint,
        IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
        ISystemClockService systemClockService,
        ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IDasServiceBusEndpoint _busEndpoint;
        private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
        private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;

        public CalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository,
            IDasServiceBusEndpoint busEndpoint,
            IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
            ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _busEndpoint = busEndpoint;
            _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
            _logger = logger;
        }

        public async Task Calculate(CalculateApprenticeshipPaymentsCommand command)
        {
            var apprenticeship = new Apprenticeship(command.EarningsGeneratedEvent);
            apprenticeship.CalculatePayments(_systemClockService.Now);
            _logger.LogInformation($"Publishing payments generated event for apprenticeship key {command.EarningsGeneratedEvent.ApprenticeshipKey}. Number of payments: {apprenticeship.Payments.Count}");

            var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
            _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

            await _apprenticeshipRepository.Add(apprenticeship);
            await _busEndpoint.Publish(@event);
        }
    }
}
