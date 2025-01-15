using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;

public class ReleasePaymentCommandHandler : ICommandHandler<ReleasePaymentCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IFinalisedOnProgammeLearningPaymentEventBuilder _eventBuilder;
    private readonly ILogger<ReleasePaymentCommandHandler> _logger;

    public ReleasePaymentCommandHandler(IApprenticeshipRepository apprenticeshipRepository, IFinalisedOnProgammeLearningPaymentEventBuilder eventBuilder, ILogger<ReleasePaymentCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _eventBuilder = eventBuilder;
        _logger = logger;
    }

    public async Task Handle(ReleasePaymentCommand command)
    {
        var apprenticeshipKey = command.ApprenticeshipKey;
        var apprenticeship = await _apprenticeshipRepository.Get(apprenticeshipKey);
        
        _logger.LogInformation("Apprenticeship Key: {apprenticeshipKey} -  Publishing payment {paymentKey}", apprenticeshipKey, command.PaymentKey);
        
        apprenticeship.SendPayment(command.PaymentKey, command.CollectionYear, command.CollectionPeriod, _eventBuilder.Build);
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}