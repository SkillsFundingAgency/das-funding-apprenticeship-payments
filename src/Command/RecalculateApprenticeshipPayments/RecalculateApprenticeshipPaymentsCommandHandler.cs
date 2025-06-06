﻿using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

public class RecalculateApprenticeshipPaymentsCommandHandler : ICommandHandler<RecalculateApprenticeshipPaymentsCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IDasServiceBusEndpoint _busEndpoint;
    private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
    private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;
    private readonly IOuterApiClient _outerApiClient;
    private readonly ISystemClockService _systemClockService;

    public RecalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipRepository apprenticeshipRepository,
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

    public async Task Handle(RecalculateApprenticeshipPaymentsCommand command)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);

        apprenticeship.Update(command.StartDate, command.PlannedEndDate, command.AgeAtStartOfApprenticeship);
        apprenticeship.ClearEarnings();
        
        foreach (var earning in command.NewEarnings)
        {
            apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth, earning.FundingLineType, earning.EarningsProfileId, earning.InstalmentType);
        }

        var academicYears = await _outerApiClient.GetAcademicYearsDetails(_systemClockService.Now);

        apprenticeship.RecalculatePayments(_systemClockService.Now, academicYears);

        var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
        _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}", @event.ApprenticeshipKey, @event.SerialiseForLogging());

        await _apprenticeshipRepository.Update(apprenticeship);
        await _busEndpoint.Publish(@event);
    }
}