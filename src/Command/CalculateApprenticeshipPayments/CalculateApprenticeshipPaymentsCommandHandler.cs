using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;

public class CalculateApprenticeshipPaymentsCommandHandler : ICommandHandler<CalculateApprenticeshipPaymentsCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ISystemClockService _systemClockService;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;
    private readonly IOuterApiClient _outerApiClient;

    public CalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository,
        IDasServiceBusEndpoint busEndpoint,
        IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
        ISystemClockService systemClockService,
        IOuterApiClient outerApiClient,
        ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _busEndpoint = busEndpoint;
        _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
        _systemClockService = systemClockService;
        _outerApiClient = outerApiClient;
        _logger = logger;
    }

    public async Task Handle(CalculateApprenticeshipPaymentsCommand command)
    {
        var exists = await _apprenticeshipRepository.Exists(command.EarningsGeneratedEvent.ApprenticeshipKey);
        if (exists)
        {
            _logger.LogInformation($"Apprenticeship {command.EarningsGeneratedEvent.ApprenticeshipKey} already exists. CalculateApprenticeshipPaymentsCommand will be ignored.");
            return;
        };

        var academicYears = await _outerApiClient.GetAcademicYearsDetails(_systemClockService.Now);

        var apprenticeship = new Apprenticeship(command.EarningsGeneratedEvent);
        apprenticeship.CalculatePayments(_systemClockService.Now, academicYears);
        _logger.LogInformation($"Publishing payments generated event for apprenticeship key {command.EarningsGeneratedEvent.ApprenticeshipKey}. Number of payments: {apprenticeship.Payments.Count}");

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _apprenticeshipRepository.Add(apprenticeship);
        await _busEndpoint.Publish(@event);
    }
}
