using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Threading.Tasks;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    public class CalculateRequiredLevyAmountFunction
    {
        private readonly ICalculateRequiredLevyAmountCommandHandler _commandHandler;

        public CalculateRequiredLevyAmountFunction(ICalculateRequiredLevyAmountCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        [FunctionName(nameof(CalculateRequiredLevyAmountFunction))]
        public async Task Run(
            [NServiceBusTrigger(Endpoint = QueueNames.FinalisedOnProgammeLearningPayment)] FinalisedOnProgammeLearningPaymentEvent @event,
            
            ILogger log)
        {
            log.LogInformation(
                $"Triggered {nameof(CalculateRequiredLevyAmountFunction)} function for Apprenticeship: {@event.ApprenticeshipKey}");

            await _commandHandler.Process(new CalculateRequiredLevyAmountCommand(@event));
        }
    }
}
