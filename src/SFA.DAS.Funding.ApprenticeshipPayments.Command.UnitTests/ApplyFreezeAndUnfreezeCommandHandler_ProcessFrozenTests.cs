using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ApplyFreezeAndUnfreezeCommandHandler_ProcessFrozenTests
{
    private Mock<IApprenticeship> _apprenticeship = null!;
    private ApplyFreezeAndUnfreezeCommand _command = null!;
    private Fixture _fixture = null!;
    private byte _collectionPeriod;
    private short _collectionYear;
    private Mock<IApprenticeshipRepository> _repository = null!;
    private Mock<ISystemClockService> _systemClockService = null!;
    private Mock<IApiClient> _apiClient = null!;
    private ApplyFreezeAndUnfreezeCommandHandler _sut = null!;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = _fixture.Create<byte>();
		_collectionYear = 2425;
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeship.SetupGet(x => x.PaymentsFrozen).Returns(true);
        _command = new ApplyFreezeAndUnfreezeCommand(_fixture.Create<Guid>(), _collectionYear, _collectionPeriod);

        _repository = new Mock<IApprenticeshipRepository>();
        _repository.Setup(x => x.Get(_command.ApprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);
        _systemClockService = new Mock<ISystemClockService>();

        _apiClient = new Mock<IApiClient>();
        _sut = new ApplyFreezeAndUnfreezeCommandHandler(_repository.Object, _systemClockService.Object, _apiClient.Object, Mock.Of<ILogger<ApplyFreezeAndUnfreezeCommandHandler>>());

        await _sut.Apply(_command);
    }

    [Test]
    public void ThenPaymentsAreMarkedAsNotPaidDueToFreeze()
    {
        _apprenticeship.Verify(x => x.MarkPaymentsAsFrozen(_collectionYear, _collectionPeriod), Times.Once);
        _repository.Verify(x => x.Update(_apprenticeship.Object), Times.Once);
    }
}