using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApprenticeshipEntity
    {
        [JsonProperty] public ApprenticeshipEntityModel Model { get; set; }

        private readonly ICalculateApprenticeshipPaymentsCommandHandler _calculateApprenticeshipPaymentsCommandHandler;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IProcessUnfundedPaymentsCommandHandler _processUnfundedPaymentsCommandHandler;
        private readonly ILogger<ApprenticeshipEntity> _logger;

        public ApprenticeshipEntity(ICalculateApprenticeshipPaymentsCommandHandler calculateApprenticeshipPaymentsCommandHandler,
            IDomainEventDispatcher domainEventDispatcher,
            IProcessUnfundedPaymentsCommandHandler processUnfundedPaymentsCommandHandler,
            ILogger<ApprenticeshipEntity> logger)
        {
            _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
            _domainEventDispatcher = domainEventDispatcher;
            _processUnfundedPaymentsCommandHandler = processUnfundedPaymentsCommandHandler;
            _logger = logger;
        }

        public async Task HandleEarningsGeneratedEvent(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            _logger.LogInformation($"Handling Earnings Generated Event For Apprenticeship Key: {earningsGeneratedEvent.ApprenticeshipKey}");
            MapEarningsGeneratedEventProperties(earningsGeneratedEvent);
            await _calculateApprenticeshipPaymentsCommandHandler.Calculate(new CalculateApprenticeshipPaymentsCommand(Model));
        }
        public async Task ReleasePaymentsForCollectionPeriod(byte collectionPeriod)
        {
            await _processUnfundedPaymentsCommandHandler.Process(new ProcessUnfundedPaymentsCommand(collectionPeriod, Model));
        }

        [FunctionName(nameof(ApprenticeshipEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<ApprenticeshipEntity>();

        private void MapEarningsGeneratedEventProperties(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            Model = new ApprenticeshipEntityModel
            {
                ApprenticeshipKey = earningsGeneratedEvent.ApprenticeshipKey,
                Earnings = earningsGeneratedEvent.DeliveryPeriods.Select(y =>
                    new EarningEntityModel
                    {
                        AcademicYear = y.AcademicYear,
                        Amount = y.LearningAmount, 
                        DeliveryPeriod = y.Period,
                        CollectionMonth = y.CalendarMonth,
                        CollectionYear = y.CalenderYear
                    }).ToList(),
                StartDate = earningsGeneratedEvent.StartDate,
                Ukprn = earningsGeneratedEvent.ProviderId,
                Uln = long.Parse(earningsGeneratedEvent.Uln),
                PlannedEndDate = earningsGeneratedEvent.ActualEndDate
            };
        }
    }
}
