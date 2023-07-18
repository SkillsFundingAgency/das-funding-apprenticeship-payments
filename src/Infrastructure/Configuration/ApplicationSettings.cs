﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Configuration;
#pragma warning disable CS8618

[ExcludeFromCodeCoverage]
public class ApplicationSettings
{
    public string AzureWebJobsStorage { get; set; }
    public string NServiceBusConnectionString { get; set; }
    public string DCServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
}

#pragma warning restore CS8618