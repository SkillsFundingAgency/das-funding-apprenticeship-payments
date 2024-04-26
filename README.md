## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# das-funding-apprenticeship-payments

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-funding-apprenticeship-payments?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3217&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-funding-apprenticeship-payments&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-funding-apprenticeship-payments)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/jira/software/c/projects/FLP/boards/753)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3480354918/Flexible+Payments+Models)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

das-funding-apprenticeship-payments is an Azure Durable Function which subscribes and publishes various events. Its purpose is to store and manage payments records raised by the creation of new apprenticeships and updated via change of circumstances events.


## How It Works

The repo contains an Azure Durable Function.
The Durable Function responds to events, and will generate/store payments data within durable entities

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports .Net6
* Azure Storage Emulator (Azureite)

### Config

Most of the application configuration is taken from the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) and the default values can be used in most cases.  The config json will need to be added to the local Azure Storage instance with a a PartitionKey of LOCAL and a RowKey of SFA.DAS.Funding.ApprenticeshipPayments_1.0.

```
{
  "ApplicationSettings":{
    "NServiceBusConnectionString":"UseLearningEndpoint=true",
    "DCServiceBusConnectionString":"UseLearningEndpoint=true",
    "NServiceBusLicense":"<LicenseKey>"
  }
}
```

## 🔗 External Dependencies

* Azure Storage Emulator (Azureite)

The durable function uses NServiceBus learning transport for local development to handle the events. More information can be found at https://docs.particular.net/transports/learning/