using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests
{
    public class ProcessUnfundedPaymentsCommandHandler_ProcessTests
    {
        private Mock<IApprenticeship> _apprenticeship = null!;
        private List<Payment> _expectedPayments = null!;
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

        [SetUp]
        public async Task Setup()
        {
            _fixture = new Fixture();
            _collectionPeriod = _fixture.Create<byte>();
            _collectionYear = 2425;
            _previousAcademicYear = 2324;
            _hardCloseDate = new DateTime(2025, 10, 15);
			
            _apprenticeship = new Mock<IApprenticeship>();
            _apprenticeship.SetupGet(x => x.PaymentsFrozen).Returns(false);
            _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _collectionYear, _fixture.Create<Guid>(), _previousAcademicYear, _hardCloseDate);

            _expectedPayments = new List<Payment>
            {
                _fixture.Create<Payment>(),
                _fixture.Create<Payment>()
            };
            _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

            _systemClockService = new Mock<ISystemClockService>();
            _systemClockService.Setup(x => x.Now).Returns(DateTime.Now);

            _busEndpoint = new Mock<IDasServiceBusEndpoint>();
            _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();

            _eventBuilder.Setup(x => x.Build(It.IsAny<Payment>(), _apprenticeship.Object)).Returns(_expectedEvent);
            _repository = new Mock<IApprenticeshipRepository>();
            _repository.Setup(x => x.Get(_command.ApprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);
            _apprenticeship.Setup(x => x.DuePayments(_collectionYear, _collectionPeriod)).Returns(_expectedPayments.AsReadOnly());
            _sut = new ProcessUnfundedPaymentsCommandHandler(_repository.Object, _busEndpoint.Object, _eventBuilder.Object, _systemClockService.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

            await _sut.Process(_command);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsReleased()
        {
            _eventBuilder.Verify(x => x.Build(_expectedPayments.First(), _apprenticeship.Object), Times.Once);
            _eventBuilder.Verify(x => x.Build(_expectedPayments.Last(), _apprenticeship.Object), Times.Once);
        }

        [Test]
        public void ThenPaymentsAreMarkedAsSent()
        {
            _apprenticeship.Verify(x => x.MarkPaymentsAsSent(_collectionYear, _collectionPeriod), Times.Once);
            _repository.Verify(x => x.Update(_apprenticeship.Object));
        }

    }
}