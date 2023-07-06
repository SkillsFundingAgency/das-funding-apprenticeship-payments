using AutoFixture;
using Moq;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests
{
    public class ProcessUnfundedPaymentsCommandHandler_ProcessTests
    {
        private ProcessUnfundedPaymentsCommand _command;
        private Fixture _fixture;
        private byte _collectionPeriod;
        private Mock<IMessageSession> _messageSession;
        private Mock<IFinalisedOnProgammeLearningPaymentEventBuilder> _eventBuilder;
        private FinalisedOnProgammeLearningPaymentEvent _expectedEvent;
        private ProcessUnfundedPaymentsCommandHandler _sut;

        [SetUp]
        public async Task Setup()
        {
            _fixture = new Fixture();
            _collectionPeriod = _fixture.Create<byte>();
            _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _fixture.Create<ApprenticeshipEntityModel>());
            _command.Model.Payments = new List<PaymentEntityModel>
            {
                new PaymentEntityModel { Amount = 100, CollectionPeriod = _collectionPeriod, SentForPayment = false }, //to be sent
                new PaymentEntityModel { Amount = 200, CollectionPeriod = _collectionPeriod, SentForPayment = true }, //already sent
                new PaymentEntityModel { Amount = 300, CollectionPeriod = (byte)(_collectionPeriod + 1), SentForPayment = false } //wrong period
            };

            _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

            _messageSession = new Mock<IMessageSession>();
            _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
            _eventBuilder.Setup(x => x.Build(It.IsAny<PaymentEntityModel>(), It.IsAny<ApprenticeshipEntityModel>())).Returns(_expectedEvent);
            _sut = new ProcessUnfundedPaymentsCommandHandler(_messageSession.Object, _eventBuilder.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

            await _sut.Process(_command);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsReleased()
        {
            _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount == 100), It.IsAny<ApprenticeshipEntityModel>()));
            _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount != 100), It.IsAny<ApprenticeshipEntityModel>()), Times.Never);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsMarkedAsSent()
        {
            _command.Model.Payments.Single(x => x.Amount == 100).SentForPayment.Should().BeTrue();
        }

    }
}