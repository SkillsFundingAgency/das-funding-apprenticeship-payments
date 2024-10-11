using SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class PaymentsFunctions
{
    private readonly IProcessUnfundedPaymentsCommandHandler _processUnfundedPaymentsCommandHandler;
    private readonly IGetApprenticeshipsWithDuePaymentsQueryHandler _getApprenticeshipsWithDuePaymentsQueryHandler;
	private readonly ISystemClockService _systemClock;
    private readonly IApiClient _apiClient;

    public PaymentsFunctions(IProcessUnfundedPaymentsCommandHandler processUnfundedPaymentsCommandHandler, IGetApprenticeshipsWithDuePaymentsQueryHandler getApprenticeshipsWithDuePaymentsQueryHandler, IApiClient apiClient, ISystemClockService systemClock)
    {
        _getApprenticeshipsWithDuePaymentsQueryHandler = getApprenticeshipsWithDuePaymentsQueryHandler;
        _processUnfundedPaymentsCommandHandler = processUnfundedPaymentsCommandHandler;
		_systemClock = systemClock;
        _apiClient = apiClient;

    [FunctionName(nameof(ReleasePaymentsEventServiceBusTrigger))]
    public async Task ReleasePaymentsEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
        ILogger log)
    {
		var previousAcademicYear = await GetPreviousAcademicYear();
		
        var result = await _getApprenticeshipsWithDuePaymentsQueryHandler.Get(new GetApprenticeshipsWithDuePaymentsQuery(releasePaymentsCommand.CollectionPeriod, releasePaymentsCommand.CollectionYear));

        var releasePaymentsTasks = result.Apprenticeships.Select(x => _processUnfundedPaymentsCommandHandler.Process(new ProcessUnfundedPaymentsCommand(releasePaymentsCommand.CollectionPeriod, releasePaymentsCommand.CollectionYear, x.ApprenticeshipKey)));
        log.LogInformation($"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} for apprenticeships. (Count: {result.Apprenticeships.Count()})");
        await Task.WhenAll(releasePaymentsTasks);

        var operationInput = new ReleasePaymentsDto
        {
            CollectionYear =  releasePaymentsCommand.CollectionYear,
            CollectionPeriod = releasePaymentsCommand.CollectionPeriod,
            PreviousAcademicYear = short.Parse(previousAcademicYear.AcademicYear),
            HardCloseDate = previousAcademicYear.HardCloseDate 
        };

        log.LogInformation($"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} complete.");
    }

    private async Task<GetAcademicYearsResponse> GetPreviousAcademicYear()
    {
        var currentAcademicYearResponse = await _apiClient.Get<GetAcademicYearsResponse>(new GetAcademicYearsRequest(_systemClock.Now));
        var lastDayOfPreviousYear = currentAcademicYearResponse.Body.StartDate.AddDays(-1);
        var previousAcademicYearResponse = await _apiClient.Get<GetAcademicYearsResponse>(new GetAcademicYearsRequest(lastDayOfPreviousYear));
        return previousAcademicYearResponse.Body;
    }
}