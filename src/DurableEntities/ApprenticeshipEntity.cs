﻿using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ResetSentForPaymentFlagForCollectionPeriod;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApprenticeshipEntity
    {
        [JsonProperty] public ApprenticeshipEntityModel Model { get; set; }

        private readonly ICalculateApprenticeshipPaymentsCommandHandler _calculateApprenticeshipPaymentsCommandHandler;
        private readonly IProcessUnfundedPaymentsCommandHandler _processUnfundedPaymentsCommandHandler;
        private readonly IRecalculateApprenticeshipPaymentsCommandHandler _recalculateApprenticeshipPaymentsCommandHandler;
        private readonly IResetSentForPaymentFlagForCollectionPeriodCommandHandler _resetSentForPaymentFlagCommandHandler;
        private readonly ILogger<ApprenticeshipEntity> _logger;

        public ApprenticeshipEntity(ICalculateApprenticeshipPaymentsCommandHandler calculateApprenticeshipPaymentsCommandHandler,
            IProcessUnfundedPaymentsCommandHandler processUnfundedPaymentsCommandHandler,
            IRecalculateApprenticeshipPaymentsCommandHandler recalculateApprenticeshipPaymentsCommandHandler,
            ILogger<ApprenticeshipEntity> logger, IResetSentForPaymentFlagForCollectionPeriodCommandHandler resetSentForPaymentFlagCommandHandler)
        {
            _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
            _processUnfundedPaymentsCommandHandler = processUnfundedPaymentsCommandHandler;
            _recalculateApprenticeshipPaymentsCommandHandler = recalculateApprenticeshipPaymentsCommandHandler;
            _logger = logger;
            _resetSentForPaymentFlagCommandHandler = resetSentForPaymentFlagCommandHandler;
        }

        public async Task HandleEarningsGeneratedEvent(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            _logger.LogInformation("ApprenticeshipKey: {0} Received EarningsGeneratedEvent: {1}",
                earningsGeneratedEvent.ApprenticeshipKey,
                earningsGeneratedEvent.SerialiseForLogging());

            _logger.LogInformation($"Handling Earnings Generated Event For Apprenticeship Key: {earningsGeneratedEvent.ApprenticeshipKey}");
            MapEarningsGeneratedEventProperties(earningsGeneratedEvent);
            await _calculateApprenticeshipPaymentsCommandHandler.Calculate(new CalculateApprenticeshipPaymentsCommand(Model));
        }

        public async Task HandleEarningsRecalculatedEvent(ApprenticeshipEarningsRecalculatedEvent earningsRecalculatedEvent)
        {
            if (IsModelNull(nameof(HandleEarningsRecalculatedEvent))) return;

            _logger.LogInformation("ApprenticeshipKey: {0} Received EarningsRecalculatedEvent: {1}",
                earningsRecalculatedEvent.ApprenticeshipKey,
                earningsRecalculatedEvent.SerialiseForLogging());

            var apprenticeship = await _recalculateApprenticeshipPaymentsCommandHandler.Recalculate(new RecalculateApprenticeshipPaymentsCommand(Model, earningsRecalculatedEvent.DeliveryPeriods.ToEarnings(earningsRecalculatedEvent.EarningsProfileId)));

            MapNewApprenticeshipValues(earningsRecalculatedEvent);
            MapNewEarningsAndPayments(apprenticeship);
        }

        public async Task ReleasePaymentsForCollectionPeriod(ReleasePaymentsDto dto)
        {
            if (IsModelNull(nameof(ReleasePaymentsForCollectionPeriod))) return;

            await _processUnfundedPaymentsCommandHandler.Process(new ProcessUnfundedPaymentsCommand(dto.CollectionPeriod, dto.CollectionYear, dto.PreviousAcademicYear, dto.HardCloseDate, Model));

        }

        public async Task ResetSentForPaymentFlagForCollectionPeriod(ResetSentForPaymentFlagForCollectionPeriodDto dto)
        {
            if (IsModelNull(nameof(ResetSentForPaymentFlagForCollectionPeriod))) return;

            _resetSentForPaymentFlagCommandHandler.Process(new ResetSentForPaymentFlagForCollectionPeriodCommand(dto.CollectionPeriod, dto.CollectionYear, Model));
        }

        public void HandlePaymentFrozenEvent(PaymentsFrozenEvent paymentsFrozenEvent)
        {
            if (IsModelNull(nameof(HandlePaymentFrozenEvent))) return;

            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} Received {eventName}",
                paymentsFrozenEvent.ApprenticeshipKey,
                nameof(PaymentsFrozenEvent));

            Model.PaymentsFrozen = true;
        }

        public void HandlePaymentsUnfrozenEvent(PaymentsUnfrozenEvent paymentsUnfrozenEvent)
        {
            if (IsModelNull(nameof(HandlePaymentsUnfrozenEvent))) return;

            _logger.LogInformation("ApprenticeshipKey: {apprenticeshipKey} Received {eventName}",
                paymentsUnfrozenEvent.ApprenticeshipKey,
                nameof(PaymentsUnfrozenEvent));

            Model.PaymentsFrozen = false;
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
                model.EarningsProfileId = earningsGeneratedEvent.EarningsProfileId;
                return model;
            }).ToList();
            Model.EmployerType = earningsGeneratedEvent.EmployerType;
            Model.StartDate = earningsGeneratedEvent.StartDate;
            Model.Ukprn = earningsGeneratedEvent.ProviderId;
            Model.Uln = long.Parse(earningsGeneratedEvent.Uln);
            Model.PlannedEndDate = earningsGeneratedEvent.PlannedEndDate;
            Model.CourseCode = earningsGeneratedEvent.TrainingCode;
            Model.FundingEmployerAccountId = earningsGeneratedEvent.EmployerAccountId;
            Model.ApprovalsApprenticeshipId = earningsGeneratedEvent.ApprovalsApprenticeshipId;
            Model.PaymentsFrozen = false;
            Model.AgeAtStartOfApprenticeship = earningsGeneratedEvent.AgeAtStartOfApprenticeship;
        }

        private void MapNewEarningsAndPayments(Apprenticeship apprenticeship)
        {
            Model.Earnings = apprenticeship.Earnings.Select(e => new EarningEntityModel
            {
                AcademicYear = e.AcademicYear,
                Amount = e.Amount,
                DeliveryPeriod = e.DeliveryPeriod,
                CollectionMonth = e.CollectionMonth,
                CollectionYear = e.CollectionYear,
                FundingLineType = e.FundingLineType,
                EarningsProfileId = e.EarningsProfileId
            }).ToList();
            Model.Payments = apprenticeship.Payments.Select(p => new PaymentEntityModel
            {
                AcademicYear = p.AcademicYear,
                Amount = p.Amount,
                CollectionPeriod = p.CollectionPeriod,
                CollectionYear = p.CollectionYear,
                DeliveryPeriod = p.DeliveryPeriod,
                FundingLineType = p.FundingLineType,
                SentForPayment = p.SentForPayment,
                EarningsProfileId = p.EarningsProfileId
            }).ToList();
        }

        private void MapNewApprenticeshipValues(ApprenticeshipEarningsRecalculatedEvent earningsRecalculatedEvent)
        {
            Model.StartDate = earningsRecalculatedEvent.StartDate;
            Model.PlannedEndDate = earningsRecalculatedEvent.PlannedEndDate;
            Model.AgeAtStartOfApprenticeship = earningsRecalculatedEvent.AgeAtStartOfApprenticeship;
        }

        private bool IsModelNull(string methodName)
        {
            if (Model == null)
            {
                _logger.LogWarning("{method} was triggered for entity with null model", methodName);
                return true;
            }

            return false;
        }
    }
}
