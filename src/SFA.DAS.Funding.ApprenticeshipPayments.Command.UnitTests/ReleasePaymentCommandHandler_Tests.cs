using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ReleasePayment;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

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

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeshipKey = Guid.NewGuid();
        _command = new ReleasePaymentCommand(_apprenticeshipKey, _fixture.Create<Guid>(), _fixture.Create<short>(), _fixture.Create<byte>());
        
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _apprenticeshipRepository.Setup(x => x.Get(_apprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);

        _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();

        _sut = new ReleasePaymentCommandHandler(
            _apprenticeshipRepository.Object,
            _eventBuilder.Object,
            Mock.Of<ILogger<ReleasePaymentCommandHandler>>());
        await _sut.Handle(_command);
    }

    [Test]
    public void ThenThePaymentIsSent()
    {
        _apprenticeship.Verify(x => x.SendPayment(_command.PaymentKey, _command.CollectionYear, _command.CollectionPeriod, _eventBuilder.Object.Build), Times.Once);
        _apprenticeshipRepository.Verify(x => x.Update(_apprenticeship.Object), Times.Once);
    }
}