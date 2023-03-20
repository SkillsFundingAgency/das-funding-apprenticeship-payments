using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    public class EarningsFunctions
    {
        [FunctionName(nameof(EarningsGeneratedEventServiceBusTrigger))]
        public async Task EarningsGeneratedEventServiceBusTrigger(
            [NServiceBusTrigger(Endpoint = QueueNames.EarningsGenerated)] EarningsGeneratedEvent apprenticeshipCreatedEvent,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation($"{nameof(EarningsGeneratedEventServiceBusTrigger)} processing...");
        }
    }
}