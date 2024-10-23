﻿using Azure;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests;

internal class TestOuterApi : IOuterApiClient
{
    public Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request)
    {
        if (request is GetLearnersInILRRequest getLearnersInIlrRequest)
        {
            return GetLearnersInILR<TResponse>(getLearnersInIlrRequest);
        }

        throw new NotImplementedException();
    }

    public Task<ApiResponse<TResponse>> GetLearnersInILR<TResponse>(GetLearnersInILRRequest request)
    {
        var json = new GetLearnersInILRResponse
        {
            AcademicYear = GetAcademicYearString(yearFrom, yearTo),
            StartDate = new DateTime(yearFrom, 8, 1),
            EndDate = new DateTime(yearTo, 7, 31),
            HardCloseDate = new DateTime(yearTo, 10, 15, 23, 59, 59)
        };

        // Check if the TResponse is assignable from the GetLearnersInILRResponse type
        if (typeof(TResponse).IsAssignableFrom(typeof(GetLearnersInILRResponse)))
        {
            return Task.FromResult(new ApiResponse<TResponse>((TResponse)(object)json, System.Net.HttpStatusCode.OK, string.Empty));
        }
        else
        {
            throw new InvalidCastException($"Cannot cast {typeof(GetLearnersInILRResponse)} to {typeof(TResponse)}");
        }
    }
}
