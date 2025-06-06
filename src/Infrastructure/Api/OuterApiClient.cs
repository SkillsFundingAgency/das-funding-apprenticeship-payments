﻿using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;

public class OuterApiClient : IOuterApiClient
{
    private readonly HttpClient _httpClient;
    private readonly PaymentsOuterApi _config;

    public OuterApiClient(HttpClient httpClient, IOptions<PaymentsOuterApi> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
        _httpClient.BaseAddress = new Uri(config.Value.BaseUrl);
    }

    public async Task<ApiResponse<TResponse>> Get<TResponse>(IGetApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        AddAuthenticationHeader(requestMessage, request);

        var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        return await ProcessResponse<TResponse>(response);
    }

    private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage, IApiRequest apiRequest)
    {
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _config.Key);
        httpRequestMessage.Headers.Add("X-Version", "1");
    }

    private static async Task<ApiResponse<TResponse>> ProcessResponse<TResponse>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var errorContent = "";
        var responseBody = default(TResponse?);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            errorContent = json;
        }
        else if (!string.IsNullOrWhiteSpace(json))
        {
            responseBody = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        var apiResponse = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);

        return apiResponse;
    }
}