using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests
{
    public class ProcessUnfundedPaymentsCommandHandler_ProcessTests
    {
        private ProcessUnfundedPaymentsCommand _command = null!;
        private Fixture _fixture = null!;
        private byte _collectionPeriod;
        private short _collectionYear;
        private Mock<IDasServiceBusEndpoint> _busEndpoint = null!;
        private Mock<IFinalisedOnProgammeLearningPaymentEventBuilder> _eventBuilder = null!;
        private Mock<ISystemClockService> _systemClockService = null!;
        private FinalisedOnProgammeLearningPaymentEvent _expectedEvent = null!;
        private ProcessUnfundedPaymentsCommandHandler _sut = null!;

        [SetUp]
        public async Task Setup()
        {
            _fixture = new Fixture();
            _collectionPeriod = _fixture.Create<byte>();
            _collectionYear = _fixture.Create<short>();
            _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _collectionYear, _fixture.Create<ApprenticeshipEntityModel>());
            _command.Model.PaymentsFrozen = false;
            _command.Model.Payments = new List<PaymentEntityModel>
            {
                new PaymentEntityModel { Amount = 100, CollectionYear = _collectionYear, CollectionPeriod = _collectionPeriod, SentForPayment = false }, //to be sent
                new PaymentEntityModel { Amount = 200, CollectionYear = _collectionYear, CollectionPeriod = _collectionPeriod, SentForPayment = true }, //already sent
                new PaymentEntityModel { Amount = 300, CollectionYear = _collectionYear, CollectionPeriod = (byte)(_collectionPeriod + 1), SentForPayment = false }, //wrong period
                new PaymentEntityModel { Amount = 400, CollectionYear = (short)(_collectionYear + 1), CollectionPeriod = _collectionPeriod, SentForPayment = false } //wrong year
            };

            _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

            _systemClockService = new Mock<ISystemClockService>();
            _systemClockService.Setup(x => x.Now).Returns(DateTime.Now);

            _busEndpoint = new Mock<IDasServiceBusEndpoint>();
            _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
            _eventBuilder.Setup(x => x.Build(It.IsAny<PaymentEntityModel>(), _command.Model)).Returns(_expectedEvent);
            _sut = new ProcessUnfundedPaymentsCommandHandler(_busEndpoint.Object, _eventBuilder.Object, _systemClockService.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

            await _sut.Process(_command);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsReleased()
        {
            _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount == 100), _command.Model));
            _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount != 100), _command.Model), Times.Never);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsMarkedAsSent()
        {
            _command.Model.Payments.Single(x => x.Amount == 100).SentForPayment.Should().BeTrue();
        }

    }
}