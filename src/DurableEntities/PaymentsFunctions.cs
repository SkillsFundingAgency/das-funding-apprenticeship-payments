using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities;

public class PaymentsFunctions
{

    private readonly ISystemClockService _systemClock;
    private readonly IApiClient _apiClient;

    public PaymentsFunctions(IApiClient apiClient, ISystemClockService systemClock)
    {
        _systemClock = systemClock;
        _apiClient = apiClient;
    }

    [FunctionName(nameof(ReleasePaymentsEventServiceBusTrigger))]
    public async Task ReleasePaymentsEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        await ReleasePayments(releasePaymentsCommand, client, log);
    }

    [FunctionName(nameof(ReleasePaymentsHttpTrigger))]
    public async Task ReleasePaymentsHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "ReleasePayments/{collectionYear}/{collectionPeriod}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        short collectionYear,
        byte collectionPeriod,
        ILogger log)
    {
        await ReleasePayments(new ReleasePaymentsCommand { CollectionPeriod = collectionPeriod, CollectionYear = collectionYear}, client, log);
    }

    [FunctionName(nameof(ResetSentForPaymentFlagForCollectionPeriodHttpTrigger))]
    public async Task ResetSentForPaymentFlagForCollectionPeriodHttpTrigger(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "ResetSentForPaymentFlagForCollectionPeriod/{collectionYear}/{collectionPeriod}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        short collectionYear,
        byte collectionPeriod,
        ILogger log)
    {
        await ResetSentForPaymentFlagForCollectionPeriod(collectionYear, collectionPeriod, client, log);
    }

    private async Task ReleasePayments(ReleasePaymentsCommand releasePaymentsCommand, IDurableEntityClient client,
        ILogger log)
    {
        using CancellationTokenSource source = new CancellationTokenSource();
        var token = source.Token;

        var previousAcademicYear = await GetPreviousAcademicYear();

        var operationInput = new ReleasePaymentsDto
        {
            CollectionYear = releasePaymentsCommand.CollectionYear,
            CollectionPeriod = releasePaymentsCommand.CollectionPeriod,
            PreviousAcademicYear = short.Parse(previousAcademicYear.AcademicYear),
            HardCloseDate = previousAcademicYear.HardCloseDate
        };

        var allApprenticeshipEntitiesQuery =
            new EntityQuery
            {
                EntityName = nameof(ApprenticeshipEntity)
            }; //default page size is 100, we may wish to tweak this in future to improve performance
        var pageCounter = 0;

        do
        {
            pageCounter++;
            await client.CleanEntityStorageAsync(true, true, token);
            var result = await client.ListEntitiesAsync(allApprenticeshipEntitiesQuery, token);
            var releasePaymentsTasks = result.Entities.Select(x => client.SignalEntityAsync(x.EntityId,
                nameof(ApprenticeshipEntity.ReleasePaymentsForCollectionPeriod), operationInput));

            allApprenticeshipEntitiesQuery.ContinuationToken = result.ContinuationToken;

            log.LogInformation(
                $"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} for page {pageCounter} of entities. (Count: {result.Entities.Count()})");
            await Task.WhenAll(releasePaymentsTasks);
        } while (allApprenticeshipEntitiesQuery.ContinuationToken != null);

        log.LogInformation(
            $"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} complete.");
    }

    private async Task ResetSentForPaymentFlagForCollectionPeriod(short collectionYear, byte collectionPeriod, IDurableEntityClient client,
        ILogger log)
    {
        using CancellationTokenSource source = new CancellationTokenSource();
        var token = source.Token;

        var operationInput = new ResetSentForPaymentFlagForCollectionPeriodDto()
        {
            CollectionYear = collectionYear,
            CollectionPeriod = collectionPeriod
        };

        var allApprenticeshipEntitiesQuery =
            new EntityQuery
            {
                EntityName = nameof(ApprenticeshipEntity)
            }; //default page size is 100
        var pageCounter = 0;

        do
        {
            pageCounter++;
            await client.CleanEntityStorageAsync(true, true, token);
            var result = await client.ListEntitiesAsync(allApprenticeshipEntitiesQuery, token);
            var resetPaymentsTasks = result.Entities.Select(x => client.SignalEntityAsync(x.EntityId,
                nameof(ApprenticeshipEntity.ResetSentForPaymentFlagForCollectionPeriod), operationInput));

            allApprenticeshipEntitiesQuery.ContinuationToken = result.ContinuationToken;

            log.LogInformation(
                $"Resetting SentForPayment flag for collection period {collectionPeriod} & year {collectionYear} for page {pageCounter} of entities. (Count: {result.Entities.Count()})");
            await Task.WhenAll(resetPaymentsTasks);
        } while (allApprenticeshipEntitiesQuery.ContinuationToken != null);

        log.LogInformation(
            $"Resetting SentForPayment flag for collection period {collectionPeriod} & year {collectionYear} complete.");
    }

    private async Task<GetAcademicYearsResponse> GetPreviousAcademicYear()
    {
        var currentAcademicYearResponse = await _apiClient.Get<GetAcademicYearsResponse>(new GetAcademicYearsRequest(_systemClock.Now));
        var lastDayOfPreviousYear = currentAcademicYearResponse.Body.StartDate.AddDays(-1);
        var previousAcademicYearResponse = await _apiClient.Get<GetAcademicYearsResponse>(new GetAcademicYearsRequest(lastDayOfPreviousYear));
        return previousAcademicYearResponse.Body;
    }
}