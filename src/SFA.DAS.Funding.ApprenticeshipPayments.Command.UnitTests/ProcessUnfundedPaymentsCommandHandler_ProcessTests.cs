using AutoFixture;
using Moq;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using FluentAssertions;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests
{
    public class ProcessUnfundedPaymentsCommandHandler_ProcessTests
    {
        private ProcessUnfundedPaymentsCommand _command;
        private Fixture _fixture;
        private byte _paymentPeriod;
        private Mock<IMessageSession> _messageSession;
        private Mock<IFinalisedOnProgammeLearningPaymentEventBuilder> _eventBuilder;
        private FinalisedOnProgammeLearningPaymentEvent _expectedEvent;
        private ProcessUnfundedPaymentsCommandHandler _sut;

        [SetUp]
        public async Task Setup()
        {
            _fixture = new Fixture();
            _paymentPeriod = _fixture.Create<byte>();
            _command = new ProcessUnfundedPaymentsCommand(_paymentPeriod, _fixture.Create<ApprenticeshipEntityModel>());
            _command.Model.Payments = new List<PaymentEntityModel>
            {
                new PaymentEntityModel { Amount = 100, PaymentPeriod = _paymentPeriod, SentForPayment = false }, //to be sent
                new PaymentEntityModel { Amount = 200, PaymentPeriod = _paymentPeriod, SentForPayment = true }, //already sent
                new PaymentEntityModel { Amount = 300, PaymentPeriod = (byte)(_paymentPeriod + 1), SentForPayment = false } //wrong period
            };

            _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

            _messageSession = new Mock<IMessageSession>();
            _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
            _eventBuilder.Setup(x => x.Build(It.IsAny<PaymentEntityModel>(), It.IsAny<Guid>())).Returns(_expectedEvent);
            _sut = new ProcessUnfundedPaymentsCommandHandler(_messageSession.Object, _eventBuilder.Object);

            await _sut.Process(_command);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsReleased()
        {
            _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount == 100), It.IsAny<Guid>()));
            _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount != 100), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public void ThenOnlyExpectedPaymentIsMarkedAsSent()
        {
            _command.Model.Payments.Single(x => x.Amount == 100).SentForPayment.Should().BeTrue();
        }

    }
}