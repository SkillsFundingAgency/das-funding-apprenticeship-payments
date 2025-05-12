## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# das-funding-apprenticeship-payments

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-funding-apprenticeship-payments?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-funding-apprenticeship-payments?branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-funding-apprenticeship-payments&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-funding-apprenticeship-payments)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/jira/software/c/projects/FLP/boards/753)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3480354918/Flexible+Payments+Models)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

das-funding-apprenticeship-payments is an Azure Function which subscribes and publishes various events. Its purpose is to store and manage payments records raised by the creation of new apprenticeships and updated via change of circumstances events.


## How It Works

The repo contains an Azure Function.
The Function responds to events, and will:
* Generate/store payments data within it's own MS SQL Server Database
* Send messages into the PV2 space containing details of payments generated which require funding

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports .Net8
* Azure Storage Emulator (Azureite)
* Local instance of SQL Server
* An instance of Azure Service Bus you can use for local development. (Developers on the payments simplification team should be assigned an instance to use)

### Config

Most of the application configuration is taken from the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) and the default values can be used in most cases.  The config json will need to be added to the local Azure Storage instance with a a PartitionKey of LOCAL and a RowKey of SFA.DAS.Funding.ApprenticeshipPayments_1.0.

### Local Running

#### Functions

* Ensure you have the following local.settings.json in the Functions project root:

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "EnvironmentName": "LOCAL",
    "ConfigNames": "SFA.DAS.Funding.ApprenticeshipPayments",
    
    "ApplicationSettings:NServiceBusConnectionString":"<your Azure service bus namespace>",
    "ApplicationSettings:DCServiceBusConnectionString":"<your Azure service bus namespace>", //In production this will be a different namespace but for development you can use the same as above.
    "ApplicationSettings:NServiceBusLicense":"<LicenseKey>",
    "ApplicationSettings:DbConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=SFA.DAS.Funding.ApprenticeshipPayments.Database;Integrated Security=true;Trusted_Connection=True;Pooling=False;Connect Timeout=30;MultipleActiveResultSets=True",
    "ApplicationSettings:ApprenticeshipsOuterApiConfiguration:Key": "",
    "ApplicationSettings:ApprenticeshipsOuterApiConfiguration:BaseUrl": "https://localhost:7101/"
  }
}
```

* Make sure Azure Storage Emulator is running
* Make sure the config above is in Azure Storage
* Deploy the database project in the solution to your local db instance (make sure the connection string in your config matches your database server)
* Make sure the Azure service bus instance exists and you have the correct namespace in the config.
* Start the SFA.DAS.Funding.ApprenticeshipPayments.Functions project

## 🔗 External Dependencies

* Azure Storage Emulator (Azureite)
* Azure service bus instance