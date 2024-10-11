using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ProcessUnfundedPaymentsCommandHandler_ProcessFrozenTests
{
    private Mock<IApprenticeship> _apprenticeship = null!;
    private ProcessUnfundedPaymentsCommand _command = null!;
    private Fixture _fixture = null!;
    private byte _collectionPeriod;
    private short _collectionYear;
    private short _previousAcademicYear;
    private DateTime _hardCloseDate;
    private Mock<IDasServiceBusEndpoint> _busEndpoint = null!;
    private Mock<IFinalisedOnProgammeLearningPaymentEventBuilder> _eventBuilder = null!;
    private Mock<IApprenticeshipRepository> _repository = null!;
    private Mock<ISystemClockService> _systemClockService = null!;
    private FinalisedOnProgammeLearningPaymentEvent _expectedEvent = null!;
    private ProcessUnfundedPaymentsCommandHandler _sut = null!;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = _fixture.Create<byte>();
		_collectionYear = 2425;
        _previousAcademicYear = 2324;
        _hardCloseDate = new DateTime(2025, 10, 15);
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeship.SetupGet(x => x.PaymentsFrozen).Returns(true);
        _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _collectionYear, _fixture.Create<Guid>(), _previousAcademicYear, _hardCloseDate);

        _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

        _busEndpoint = new Mock<IDasServiceBusEndpoint>();
        _repository = new Mock<IApprenticeshipRepository>();
        _repository.Setup(x => x.Get(_command.ApprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);
        _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
        _systemClockService = new Mock<ISystemClockService>();
        _systemClockService.Setup(x => x.Now).Returns(DateTime.Now);

        _eventBuilder.Setup(x => x.Build(It.IsAny<PaymentEntityModel>(), _command.Model)).Returns(_expectedEvent);
        _sut = new ProcessUnfundedPaymentsCommandHandler(_repository.Object, _busEndpoint.Object, _eventBuilder.Object, _systemClockService.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

        await _sut.Process(_command);
    }

    [Test]
    public void ThenNoPaymentIsReleased()
    {
        _eventBuilder.Verify(x => x.Build(It.IsAny<Domain.Apprenticeship.Payment>(), It.IsAny<Domain.Apprenticeship.Apprenticeship>()), Times.Never());
    }

    [Test]
    public void ThenPaymentsAreMarkedAsNotPaidDueToFreeze()
    {
        _apprenticeship.Verify(x => x.MarkPaymentsAsFrozen(_collectionYear, _collectionPeriod), Times.Once);
        _repository.Verify(x => x.Update(_apprenticeship.Object), Times.Once);
    }
}