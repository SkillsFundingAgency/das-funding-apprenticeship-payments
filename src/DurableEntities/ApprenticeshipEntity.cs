﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus.Logging;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.NServiceBus;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            _logger.LogInformation("ApprenticeshipKey: {0} Received EarningsGeneratedEvent: {1}",
                earningsGeneratedEvent.ApprenticeshipKey,
                JsonSerializer.Serialize(earningsGeneratedEvent, new JsonSerializerOptions { WriteIndented = true }));

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
            Model = new ApprenticeshipEntityModel();
            Model.ApprenticeshipKey = earningsGeneratedEvent.ApprenticeshipKey;
            Model.Earnings = earningsGeneratedEvent.DeliveryPeriods.Select(y =>
            {
                var model = new EarningEntityModel();
                model.AcademicYear = y.AcademicYear;
                model.Amount = y.LearningAmount;
                model.DeliveryPeriod = y.Period;
                model.CollectionMonth = y.CalendarMonth;
                model.CollectionYear = y.CalenderYear;
                model.FundingLineType = y.FundingLineType;
                return model;
            }).ToList();
            Model.StartDate = earningsGeneratedEvent.StartDate;
            Model.Ukprn = earningsGeneratedEvent.ProviderId;
            Model.Uln = long.Parse(earningsGeneratedEvent.Uln);
            Model.PlannedEndDate = earningsGeneratedEvent.PlannedEndDate;
            Model.CourseCode = earningsGeneratedEvent.TrainingCode;
            Model.FundingEmployerAccountId = earningsGeneratedEvent.EmployerAccountId;
            Model.ApprovalsApprenticeshipId = earningsGeneratedEvent.ApprovalsApprenticeshipId;
        }
    }
}
