using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;

public class ReleasePaymentCommandHandler : ICommandHandler<ReleasePaymentCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;
    private readonly ILogger<ReleasePaymentCommandHandler> _logger;
    private readonly IDasServiceBusEndpoint _busEndpoint;

    public ReleasePaymentCommandHandler(IApprenticeshipRepository apprenticeshipRepository, IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder, ILogger<ReleasePaymentCommandHandler> logger, IDasServiceBusEndpoint busEndpoint)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _eventBuilder = eventBuilder;
        _logger = logger;
        _busEndpoint = busEndpoint;
    }

    public async Task Handle(ReleasePaymentCommand command)
    {
        var apprenticeshipKey = command.ApprenticeshipKey;
        var apprenticeship = await _apprenticeshipRepository.Get(apprenticeshipKey);
        
        _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  Publishing payment {paymentKey}", apprenticeshipKey, command.PaymentKey);
        
        var payment = apprenticeship.SendPayment(command.PaymentKey, command.CollectionYear, command.CollectionPeriod);
        await _apprenticeshipRepository.Update(apprenticeship);
        await _busEndpoint.Publish(_eventBuilder.Build(payment, apprenticeship));
    }
}