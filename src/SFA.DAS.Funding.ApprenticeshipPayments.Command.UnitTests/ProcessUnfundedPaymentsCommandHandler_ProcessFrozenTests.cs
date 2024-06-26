using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ProcessUnfundedPaymentsCommandHandler_ProcessFrozenTests
{
    private ProcessUnfundedPaymentsCommand _command = null!;
    private Fixture _fixture = null!;
    private byte _collectionPeriod;
    private short _collectionYear;
    private Mock<IDasServiceBusEndpoint> _busEndpoint = null!;
    private Mock<IFinalisedOnProgammeLearningPaymentEventBuilder> _eventBuilder = null!;
    private FinalisedOnProgammeLearningPaymentEvent _expectedEvent = null!;
    private ProcessUnfundedPaymentsCommandHandler _sut = null!;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = _fixture.Create<byte>();
        _collectionYear = _fixture.Create<short>();
        _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _collectionYear, _fixture.Create<ApprenticeshipEntityModel>());
        _command.Model.PaymentsFrozen = true;
        _command.Model.Payments = new List<PaymentEntityModel>
        {
            new PaymentEntityModel { Amount = 100, CollectionYear = _collectionYear, CollectionPeriod = _collectionPeriod, SentForPayment = false }
        };

        _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

        _busEndpoint = new Mock<IDasServiceBusEndpoint>();
        _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
        _eventBuilder.Setup(x => x.Build(It.IsAny<PaymentEntityModel>(), _command.Model)).Returns(_expectedEvent);
        _sut = new ProcessUnfundedPaymentsCommandHandler(_busEndpoint.Object, _eventBuilder.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

        await _sut.Process(_command);
    }

    [Test]
    public void ThenNoPaymentIsReleased()
    {
        _eventBuilder.Verify(x => x.Build(It.IsAny<PaymentEntityModel>(), It.IsAny<ApprenticeshipEntityModel>()), Times.Never());
    }

    [Test]
    public void ThenExpectedPaymentIsMarkedAsNotPaidDueToFreeze()
    {
        _command.Model.Payments.Single(x => x.Amount == 100).NotPaidDueToFreeze.Should().BeTrue();
    }

    [Test]
    public void ThenExpectedPaymentIsMarkedAsNotSentForPayment()
    {
        _command.Model.Payments.Single(x => x.Amount == 100).SentForPayment.Should().BeFalse();
    }

}