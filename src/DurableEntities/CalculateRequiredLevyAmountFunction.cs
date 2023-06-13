using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Threading.Tasks;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    public class CalculateRequiredLevyAmountFunction
    {
        [FunctionName(nameof(CalculateRequiredLevyAmountFunction))]
        public async Task Run(
            [NServiceBusTrigger(Endpoint = QueueNames.FinalisedOnProgammeLearningPayment)] FinalisedOnProgammeLearningPaymentEvent @event,
            ICalculateRequiredLevyAmountCommandHandler commandHandler,
            ILogger log)
        {
            log.LogInformation(
                $"Triggered {nameof(CalculateRequiredLevyAmountFunction)} function for Apprenticeship: {@event.ApprenticeshipKey}");

            await commandHandler.Process(new CalculateRequiredLevyAmountCommand(@event));
        }
    }
}
