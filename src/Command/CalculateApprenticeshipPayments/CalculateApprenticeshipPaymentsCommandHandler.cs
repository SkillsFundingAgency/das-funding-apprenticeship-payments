using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments
{
    public class CalculateApprenticeshipPaymentsCommandHandler : ICalculateApprenticeshipPaymentsCommandHandler
    {
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IMessageSession _messageSession;
        private readonly IPaymentsGeneratedEventBuilder _paymentsGeneratedEventBuilder;
        private readonly ILogger<CalculateApprenticeshipPaymentsCommandHandler> _logger;

        public CalculateApprenticeshipPaymentsCommandHandler(IApprenticeshipFactory apprenticeshipFactory,
            IMessageSession messageSession,
            IPaymentsGeneratedEventBuilder paymentsGeneratedEventBuilder,
            ILogger<CalculateApprenticeshipPaymentsCommandHandler> logger)
        {
            _apprenticeshipFactory = apprenticeshipFactory;
            _messageSession = messageSession;
            _paymentsGeneratedEventBuilder = paymentsGeneratedEventBuilder;
            _logger = logger;
        }

        public async Task<Apprenticeship> Calculate(CalculateApprenticeshipPaymentsCommand command)
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew(command.ApprenticeshipEntity);
            apprenticeship.CalculatePayments(DateTime.Now);
            command.ApprenticeshipEntity.Payments = MapPaymentsToModel(apprenticeship.Payments);
            _logger.LogInformation($"Publishing payments generated event for apprenticeship key {command.ApprenticeshipEntity.ApprenticeshipKey}. Number of payments: {command.ApprenticeshipEntity.Payments.Count}");

            var @event = _paymentsGeneratedEventBuilder.Build(apprenticeship);
            _logger.LogInformation("ApprenticeshipKey: {0} Publishing PaymentsGeneratedEvent: {1}",
                @event.ApprenticeshipKey,
                JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = true }));

            await _messageSession.Publish(@event);
            return apprenticeship;
        }

        private List<PaymentEntityModel> MapPaymentsToModel(IReadOnlyCollection<Payment> apprenticeshipPayments)
        {
            return apprenticeshipPayments.Select(x => new PaymentEntityModel
            {
                CollectionYear = x.CollectionYear,
                AcademicYear = x.AcademicYear,
                Amount = x.Amount,
                DeliveryPeriod = x.DeliveryPeriod,
                CollectionPeriod = x.CollectionPeriod,
                SentForPayment = x.SentForPayment
            }).ToList();
        }
    }
}
