using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions
{
    public class EarningsFunctions
    {
        private readonly ICalculateApprenticeshipPaymentsCommandHandler _calculateApprenticeshipPaymentsCommandHandler;
        private readonly IRecalculateApprenticeshipPaymentsCommandHandler _recalculateApprenticeshipPaymentsCommandHandler;

        public EarningsFunctions(ICalculateApprenticeshipPaymentsCommandHandler calculateApprenticeshipPaymentsCommandHandler, IRecalculateApprenticeshipPaymentsCommandHandler recalculateApprenticeshipPaymentsCommandHandler)
        {
            _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
            _recalculateApprenticeshipPaymentsCommandHandler = recalculateApprenticeshipPaymentsCommandHandler;
        }

        [FunctionName(nameof(EarningsGeneratedEventServiceBusTrigger))]
        public async Task EarningsGeneratedEventServiceBusTrigger(
            [NServiceBusTrigger(Endpoint = QueueNames.EarningsGenerated)] EarningsGeneratedEvent earningsGeneratedEvent,
            ILogger log)
        {
            await _calculateApprenticeshipPaymentsCommandHandler.Calculate(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
        }

        [FunctionName(nameof(EarningsGeneratedEventHttpTrigger))]
        public async Task EarningsGeneratedEventHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest request,
            ILogger log)
        {
            var earningsGeneratedEvent = new EarningsGeneratedEvent { ApprenticeshipKey = Guid.NewGuid() };
            await _calculateApprenticeshipPaymentsCommandHandler.Calculate(new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent));
        }

        [FunctionName(nameof(EarningsRecalculatedEventServiceBusTrigger))]
        public async Task EarningsRecalculatedEventServiceBusTrigger(
            [NServiceBusTrigger(Endpoint = QueueNames.EarningsRecalculated)] ApprenticeshipEarningsRecalculatedEvent earningsRecalculatedEvent,
            ILogger log)
        {
            await _recalculateApprenticeshipPaymentsCommandHandler.Recalculate(
                new RecalculateApprenticeshipPaymentsCommand(earningsRecalculatedEvent.ApprenticeshipKey,
                    earningsRecalculatedEvent.DeliveryPeriods.ToEarnings(earningsRecalculatedEvent.ApprenticeshipKey,
                        earningsRecalculatedEvent.EarningsProfileId)));
        }
    }
}