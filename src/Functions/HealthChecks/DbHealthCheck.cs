using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.HealthChecks;

[ExcludeFromCodeCoverage]
internal class DbHealthCheck : BaseHealthCheck<DbHealthCheck>
{
    private readonly string _connectionString;

    internal DbHealthCheck(string connectionString, ILogger<DbHealthCheck> logger) : base(logger)
    {
        _connectionString = connectionString;
    }

    protected override async Task<HealthCheckResult> HealthCheck(CancellationToken cancellationToken)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
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


