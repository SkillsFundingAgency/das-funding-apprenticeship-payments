using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ReleasePaymentCommandHandler_Tests
{
    private ReleasePaymentCommand _command = null!;
    private Fixture _fixture = null!;
    private ReleasePaymentCommandHandler _sut = null!;
    private Mock<IApprenticeship> _apprenticeship = null!;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Guid _apprenticeshipKey;
    private Mock<IFinalisedOnProgammeLearningPaymentEventBuilder> _eventBuilder;
    private Mock<IDasServiceBusEndpoint> _busEndpoint;
    private FinalisedOnProgammeLearningPaymentEvent _paymentEvent;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeshipKey = Guid.NewGuid();
        _command = new ReleasePaymentCommand(_apprenticeshipKey, _fixture.Create<Guid>(), _fixture.Create<short>(), _fixture.Create<byte>());
        
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _apprenticeshipRepository.Setup(x => x.Get(_apprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);

        _paymentEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
        _eventBuilder
            .Setup(x => x.Build(It.IsAny<Payment>(), It.IsAny<IApprenticeship>()))
            .Returns(_paymentEvent);

        _busEndpoint = new Mock<IDasServiceBusEndpoint>();

        _sut = new ReleasePaymentCommandHandler(
            _apprenticeshipRepository.Object,
            _eventBuilder.Object,
            Mock.Of<ILogger<ReleasePaymentCommandHandler>>(),
            _busEndpoint.Object
            );
        await _sut.Handle(_command);
    }

    [Test]
    public void ThenThePaymentIsSent()
    {
        _apprenticeship.Verify(x => x.SendPayment(_command.PaymentKey, _command.CollectionYear, _command.CollectionPeriod), Times.Once);
        _apprenticeshipRepository.Verify(x => x.Update(_apprenticeship.Object), Times.Once);
        _busEndpoint.Verify(x => x.Publish(_paymentEvent));
    }
}