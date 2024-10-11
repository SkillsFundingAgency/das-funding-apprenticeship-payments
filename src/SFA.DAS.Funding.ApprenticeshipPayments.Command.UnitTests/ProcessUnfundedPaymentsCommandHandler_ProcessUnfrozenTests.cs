using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ProcessUnfundedPaymentsCommandHandler_ProcessUnfrozenTests
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
    private Mock<ISystemClockService> _systemClockService = null!;
    private FinalisedOnProgammeLearningPaymentEvent _expectedEvent = null!;
    private Mock<IApprenticeshipRepository> _repository = null!;
    private ProcessUnfundedPaymentsCommandHandler _sut = null!;
    private DateTime _expectedCurrentDate;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = 4;
        _collectionYear = 2425;
        _previousAcademicYear = 2324;
        _hardCloseDate = new DateTime(2025,10,15);
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeship.SetupGet(x => x.PaymentsFrozen).Returns(false);
        _apprenticeship.Setup(x => x.DuePayments(_collectionYear, _collectionPeriod)).Returns(new List<Domain.Apprenticeship.Payment>().AsReadOnly());
        _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _collectionYear, _fixture.Create<Guid>(), _previousAcademicYear, _hardCloseDate);
        _systemClockService = new Mock<ISystemClockService>();
        _expectedCurrentDate = new DateTime(2024, 11, 15);
        _systemClockService.Setup(x => x.Now).Returns(_expectedCurrentDate);

        _busEndpoint = new Mock<IDasServiceBusEndpoint>();
        _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();

        _eventBuilder.Setup(x => x.Build(It.IsAny<Domain.Apprenticeship.Payment>(), It.IsAny<Domain.Apprenticeship.Apprenticeship>())).Returns(_expectedEvent);
        _repository = new Mock<IApprenticeshipRepository>();
        _repository.Setup(x => x.Get(_command.ApprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);
        _sut = new ProcessUnfundedPaymentsCommandHandler(_repository.Object, _busEndpoint.Object, _eventBuilder.Object, _systemClockService.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

        await _sut.Process(_command);
    }

    [Test]
    public void ThenPreviouslyFrozenPaymentAreUnfrozen()
    {
        _apprenticeship.Verify(x => x.UnfreezeFrozenPayments(_collectionYear, _collectionPeriod, _collectionYear, _previousAcademicYear, _hardCloseDate, _expectedCurrentDate), Times.Once);
    }
}