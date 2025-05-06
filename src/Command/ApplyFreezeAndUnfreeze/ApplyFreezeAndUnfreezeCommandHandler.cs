using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;

public class ApplyFreezeAndUnfreezeCommandHandler : ICommandHandler<ApplyFreezeAndUnfreezeCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly ISystemClockService _systemClock;
    private readonly IOuterApiClient _outerApiClient;
    private readonly ILogger<ApplyFreezeAndUnfreezeCommandHandler> _logger;

    public ApplyFreezeAndUnfreezeCommandHandler(IApprenticeshipRepository apprenticeshipRepository, ISystemClockService systemClock, IOuterApiClient outerApiClient, ILogger<ApplyFreezeAndUnfreezeCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _systemClock = systemClock;
        _outerApiClient = outerApiClient;
        _logger = logger;
    }

    public async Task Handle(ApplyFreezeAndUnfreezeCommand command)
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
            var academicYears = await _outerApiClient.GetAcademicYearsDetails(_systemClock.Now);
            var previousAcademicYear = academicYears.PreviousYear;
            apprenticeship.UnfreezeFrozenPayments(_systemClock.Now.ToAcademicYear(), previousAcademicYear.AcademicYear, previousAcademicYear.HardCloseDate, _systemClock.Now);
            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} - Any frozen frozen payments have been defrosted.", apprenticeshipKey);
        }

        await _apprenticeshipRepository.Update(apprenticeship);
    }

}