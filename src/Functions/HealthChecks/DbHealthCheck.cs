using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;

[ExcludeFromCodeCoverage]
internal class DbHealthCheck : BaseHealthCheck<DbHealthCheck>
{
    private readonly string _connectionString;
    private readonly ISqlAzureIdentityTokenProvider? _tokenProvider;

    internal DbHealthCheck(string connectionString, ILogger<DbHealthCheck> logger, ISqlAzureIdentityTokenProvider? tokenProvider) : base(logger)
    {
        _connectionString = connectionString;
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HealthCheckResult> HealthCheck(CancellationToken cancellationToken)
    {
        try
        {

            using var connection = new SqlConnection(_connectionString);

            if (_tokenProvider != null)
            {
                var token = await _tokenProvider.GetAccessTokenAsync();
                connection.AccessToken = token;
            }


            await connection.OpenAsync(cancellationToken);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("Database connection is OK.");
        }
        catch (Exception ex)
        {
            LogError("Database connection failed.", ex);
            return HealthCheckResult.Unhealthy("Database connection failed.");
        }
    }
}