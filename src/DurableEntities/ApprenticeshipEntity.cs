using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
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

        public ApprenticeshipEntity(ICalculateApprenticeshipPaymentsCommandHandler calculateApprenticeshipPaymentsCommandHandler, IDomainEventDispatcher domainEventDispatcher)
        {
            _calculateApprenticeshipPaymentsCommandHandler = calculateApprenticeshipPaymentsCommandHandler;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task HandleEarningsGeneratedEvent(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            MapEarningsGeneratedEventProperties(earningsGeneratedEvent);
            var apprenticeship = await _calculateApprenticeshipPaymentsCommandHandler.Calculate(new CalculateApprenticeshipPaymentsCommand(Model));
            Model.Payments = MapPaymentsToModel(apprenticeship.Payments);
        }

        [FunctionName(nameof(ApprenticeshipEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<ApprenticeshipEntity>();

        private void MapEarningsGeneratedEventProperties(EarningsGeneratedEvent earningsGeneratedEvent)
        {
            Model = new ApprenticeshipEntityModel
            {
                ApprenticeshipKey = earningsGeneratedEvent.ApprenticeshipKey,
                Earnings = earningsGeneratedEvent.FundingPeriods.SelectMany(x => x.DeliveryPeriods).Select(y =>
                    new EarningEntityModel
                    {
                        AcademicYear = y.AcademicYear,
                        Amount = y.LearningAmount, 
                        DeliveryPeriod = y.Period,
                        CollectionMonth = y.CalendarMonth,
                        CollectionYear = y.CalenderYear
                    }).ToList()
            };
        }

        private List<PaymentEntityModel> MapPaymentsToModel(IReadOnlyCollection<Payment> apprenticeshipPayments)
        {
            return apprenticeshipPayments.Select(x => new PaymentEntityModel
            {
                PaymentYear = x.PaymentYear,
                AcademicYear = x.AcademicYear,
                Amount = x.Amount,
                DeliveryPeriod = x.DeliveryPeriod,
                PaymentPeriod = x.PaymentPeriod,
                SentForPayment = x.SentForPayment
            }).ToList();
        }
    }
}
