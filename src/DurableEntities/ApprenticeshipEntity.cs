using Newtonsoft.Json;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApprenticeshipEntity
    {
        [JsonProperty] public ApprenticeshipEntityModel Model { get; set; }

        private readonly ICalculateApprenticeshipPaymentsCommandHandler _calculateApprenticeshipPaymentsCommandHandler;
        private readonly IProcessUnfundedPaymentsCommandHandler _processUnfundedPaymentsCommandHandler;
        private readonly IRecalculateApprenticeshipPaymentsCommandHandler _recalculateApprenticeshipPaymentsCommandHandler;
        private readonly ILogger<ApprenticeshipEntity> _logger;

        public ApprenticeshipEntity(ICalculateApprenticeshipPaymentsCommandHandler calculateApprenticeshipPaymentsCommandHandler,
            IProcessUnfundedPaymentsCommandHandler processUnfundedPaymentsCommandHandler,
            IRecalculateApprenticeshipPaymentsCommandHandler recalculateApprenticeshipPaymentsCommandHandler,
            ILogger<ApprenticeshipEntity> logger)
        {
            _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
            _processUnfundedPaymentsCommandHandler = processUnfundedPaymentsCommandHandler;
            _recalculateApprenticeshipPaymentsCommandHandler = recalculateApprenticeshipPaymentsCommandHandler;
            _logger = logger;
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
            _logger.LogInformation("ApprenticeshipKey: {0} Received EarningsRecalculatedEvent: {1}",
                earningsRecalculatedEvent.ApprenticeshipKey,
                earningsRecalculatedEvent.SerialiseForLogging());

            var apprenticeship = await _recalculateApprenticeshipPaymentsCommandHandler.Recalculate(new RecalculateApprenticeshipPaymentsCommand(Model, earningsRecalculatedEvent.DeliveryPeriods.ToEarnings()));

            MapNewEarningsAndPayments(apprenticeship);
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
            Model.EmployerType = earningsGeneratedEvent.EmployerType;
            Model.StartDate = earningsGeneratedEvent.StartDate;
            Model.Ukprn = earningsGeneratedEvent.ProviderId;
            Model.Uln = long.Parse(earningsGeneratedEvent.Uln);
            Model.PlannedEndDate = earningsGeneratedEvent.PlannedEndDate;
            Model.CourseCode = earningsGeneratedEvent.TrainingCode;
            Model.FundingEmployerAccountId = earningsGeneratedEvent.EmployerAccountId;
            Model.ApprovalsApprenticeshipId = earningsGeneratedEvent.ApprovalsApprenticeshipId;
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
                FundingLineType = e.FundingLineType
            }).ToList();
            Model.Payments = apprenticeship.Payments.Select(p => new PaymentEntityModel
            {
                AcademicYear = p.AcademicYear,
                Amount = p.Amount,
                CollectionPeriod = p.CollectionPeriod,
                CollectionYear = p.CollectionYear,
                DeliveryPeriod = p.DeliveryPeriod,
                FundingLineType = p.FundingLineType,
                SentForPayment = p.SentForPayment
            }).ToList();
        }
    }
}
