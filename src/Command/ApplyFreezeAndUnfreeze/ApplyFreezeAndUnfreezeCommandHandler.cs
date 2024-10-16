using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;

public class ApplyFreezeAndUnfreezeCommandHandler : IApplyFreezeAndUnfreezeCommandHandler
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly ISystemClockService _systemClock;
    private readonly IApiClient _apiClient;
    private readonly ILogger<ApplyFreezeAndUnfreezeCommandHandler> _logger;

    public ApplyFreezeAndUnfreezeCommandHandler(IApprenticeshipRepository apprenticeshipRepository, ISystemClockService systemClock, IApiClient apiClient, ILogger<ApplyFreezeAndUnfreezeCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _systemClock = systemClock;
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task Apply(ApplyFreezeAndUnfreezeCommand command)
    {
        var apprenticeshipKey = command.ApprenticeshipKey;
        var apprenticeship = await _apprenticeshipRepository.Get(apprenticeshipKey);
        
        if (apprenticeship.PaymentsFrozen)
        {
            apprenticeship.MarkPaymentsAsFrozen(command.CollectionYear, command.CollectionPeriod);
            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} - Payments are frozen, no payments will be published", apprenticeshipKey);
        }
        else
        {
            var previousAcademicYear = await GetPreviousAcademicYear();
            apprenticeship.UnfreezeFrozenPayments(command.CollectionYear, command.CollectionPeriod, _systemClock.Now.ToAcademicYear(), short.Parse(previousAcademicYear.AcademicYear), previousAcademicYear.HardCloseDate, _systemClock.Now);
        }

        await _apprenticeshipRepository.Update(apprenticeship);
    }

    private async Task<GetAcademicYearsResponse> GetPreviousAcademicYear()
    {
        var currentAcademicYearResponse = await _apiClient.Get<GetAcademicYearsResponse>(new GetAcademicYearsRequest(_systemClock.Now));
        var lastDayOfPreviousYear = currentAcademicYearResponse.Body.StartDate.AddDays(-1);
        var previousAcademicYearResponse = await _apiClient.Get<GetAcademicYearsResponse>(new GetAcademicYearsRequest(lastDayOfPreviousYear));
        return previousAcademicYearResponse.Body;
    }
}