﻿using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;

[ExcludeFromCodeCoverage]
public class SqlAzureIdentityAuthenticationDbConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ILogger<SqlAzureIdentityAuthenticationDbConnectionInterceptor> _logger;
    private readonly ISqlAzureIdentityTokenProvider _tokenProvider;
    private static bool _connectionNeedsAccessToken = true;

    public SqlAzureIdentityAuthenticationDbConnectionInterceptor(ILogger<SqlAzureIdentityAuthenticationDbConnectionInterceptor>? logger, ISqlAzureIdentityTokenProvider? tokenProvider, bool connectionNeedsAccessToken)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _connectionNeedsAccessToken = connectionNeedsAccessToken;
    }

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        var sqlConnection = (SqlConnection)connection;
        if (_connectionNeedsAccessToken)
        {
            _logger.LogInformation("Getting AccessToken");
            var token = _tokenProvider.GetAccessToken();
            sqlConnection.AccessToken = token;
        }

        return base.ConnectionOpening(connection, eventData, result);
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        var sqlConnection = (SqlConnection)connection;
        if (_connectionNeedsAccessToken)
        {
            _logger.LogInformation("Getting AccessToken Async");

            var token = await _tokenProvider.GetAccessTokenAsync();

            sqlConnection.AccessToken = token;
        }

        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }
}