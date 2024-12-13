using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DataAccess;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkForApprenticeships(this IServiceCollection services, ApplicationSettings settings, bool connectionNeedsAccessToken)
    {
        services.AddSingleton<ISqlAzureIdentityTokenProvider, SqlAzureIdentityTokenProvider>();

        services.AddSingleton(provider => new SqlAzureIdentityAuthenticationDbConnectionInterceptor(provider.GetService<ILogger<SqlAzureIdentityAuthenticationDbConnectionInterceptor>>(), provider.GetService<ISqlAzureIdentityTokenProvider>(), connectionNeedsAccessToken));

        services.AddScoped(p =>
        {
            var options = new DbContextOptionsBuilder<ApprenticeshipPaymentsDataContext>()
                .UseSqlServer(new SqlConnection(settings.DbConnectionString), optionsBuilder => optionsBuilder.CommandTimeout(7200)) //7200=2hours
                .AddInterceptors(p.GetRequiredService<SqlAzureIdentityAuthenticationDbConnectionInterceptor>())
                .Options;
            return new ApprenticeshipPaymentsDataContext(options);
        });

        services.AddScoped<IApprenticeshipRepository, ApprenticeshipRepository>();
        services.AddScoped<IApprenticeshipQueryRepository, ApprenticeshipQueryRepository>();

        return services.AddScoped(provider =>
        {
            var dataContext = provider.GetService<ApprenticeshipPaymentsDataContext>() ?? throw new ArgumentNullException("ApprenticeshipPaymentsDataContext");
            return new Lazy<ApprenticeshipPaymentsDataContext>(dataContext);
        });
    }
}