using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ProcessUnfundedPaymentsCommandHandler_ProcessUnfrozenTests
{
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
    private ProcessUnfundedPaymentsCommandHandler _sut = null!;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = 4;
        _collectionYear = 2425;
        _previousAcademicYear = 2324;
        _hardCloseDate = new DateTime(2025,10,15);

        _command = new ProcessUnfundedPaymentsCommand(_collectionPeriod, _collectionYear, _previousAcademicYear, _hardCloseDate, _fixture.Create<ApprenticeshipEntityModel>());
        _command.Model.PaymentsFrozen = false;
        _command.Model.Payments = new List<PaymentEntityModel>
        {
            new PaymentEntityModel { Amount = 100, CollectionYear = _collectionYear, CollectionPeriod = (byte)(_collectionPeriod-1), SentForPayment = false, NotPaidDueToFreeze = true },
            new PaymentEntityModel { Amount = 200, CollectionYear = _collectionYear, CollectionPeriod = _collectionPeriod, SentForPayment = false }
        };

        _expectedEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

        _systemClockService = new Mock<ISystemClockService>();
        _systemClockService.Setup(x => x.Now).Returns(new DateTime(2024, 11, 15));

        _busEndpoint = new Mock<IDasServiceBusEndpoint>();
        _eventBuilder = new Mock<IFinalisedOnProgammeLearningPaymentEventBuilder>();
        _eventBuilder.Setup(x => x.Build(It.IsAny<PaymentEntityModel>(), _command.Model)).Returns(_expectedEvent);
        _sut = new ProcessUnfundedPaymentsCommandHandler(_busEndpoint.Object, _eventBuilder.Object, _systemClockService.Object, Mock.Of<ILogger<ProcessUnfundedPaymentsCommandHandler>>());

        await _sut.Process(_command);
    }

    [Test]
    public void ThenPreviouslyFrozenPaymentIsReleased()
    {
        _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount == 100 && y.CollectionPeriod == _collectionPeriod), _command.Model));
    }

    [Test]
    public void ThenCurrentCollectionPeriodPaymentIsReleased()
    {
        _eventBuilder.Verify(x => x.Build(It.Is<PaymentEntityModel>(y => y.Amount == 200), _command.Model));
    }

    [Test]
    public void ThenExpectedPaymentIsMarkedCorrectly()
    {
        _command.Model.Payments.Single(x => x.Amount == 100).NotPaidDueToFreeze.Should().BeFalse();
        _command.Model.Payments.Single(x => x.Amount == 100).SentForPayment.Should().BeTrue();
    }
}