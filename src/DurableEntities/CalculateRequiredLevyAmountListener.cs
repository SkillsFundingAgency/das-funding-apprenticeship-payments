//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
//using SFA.DAS.NServiceBus.AzureFunction.Attributes;
//using SFA.DAS.Payments.RequiredPayments.Messages.Events;
//using System.Threading.Tasks;

//namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
//{
//    public class CalculateRequiredLevyAmountListener
//    {
//        [FunctionName(nameof(CalculateRequiredLevyAmountListener))]
//        public async Task Run(
//            [NServiceBusTrigger(Endpoint = QueueNames.CalculatedRequiredLevyAmount)] CalculatedRequiredLevyAmount @event,
//            ILogger log)
//        {
//            log.LogInformation(
//                $"Triggered {nameof(CalculateRequiredLevyAmountListener)} function for ApprenticeshipId: {@event.ApprenticeshipId}");

//        }
//    }
//}
