using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ResetSentForPaymentFlagForCollectionPeriod;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ResetSentForPaymentFlagForCollectionPeriodCommandHandler_ProcessTests
{
    private ResetSentForPaymentFlagForCollectionPeriodCommand _command = null!;
    private Fixture _fixture = null!;
    private byte _collectionPeriod;
    private short _collectionYear;
    private ResetSentForPaymentFlagForCollectionPeriodCommandHandler _sut = null!;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = _fixture.Create<byte>();
        _collectionYear = 2425;
        _command = new ResetSentForPaymentFlagForCollectionPeriodCommand(_collectionPeriod, _collectionYear, _fixture.Create<ApprenticeshipEntityModel>());
        _command.Model.Payments = new List<PaymentEntityModel>
        {
            new PaymentEntityModel { Amount = 100, CollectionYear = _collectionYear, CollectionPeriod = _collectionPeriod, SentForPayment = false }, //not sent for collection period, should remain false
            new PaymentEntityModel { Amount = 200, CollectionYear = _collectionYear, CollectionPeriod = _collectionPeriod, SentForPayment = true }, //should be reset
            new PaymentEntityModel { Amount = 300, CollectionYear = _collectionYear, CollectionPeriod = (byte)(_collectionPeriod + 1), SentForPayment = true }, //wrong period no reset
            new PaymentEntityModel { Amount = 400, CollectionYear = (short)(_collectionYear + 1), CollectionPeriod = _collectionPeriod, SentForPayment = true } //wrong year no reset
        };

        _sut = new ResetSentForPaymentFlagForCollectionPeriodCommandHandler(Mock.Of<ILogger<ResetSentForPaymentFlagForCollectionPeriodCommandHandler>>());

        _sut.Process(_command);
    }

    [Test]
    public void ThenOnlyExpectedPaymentsAreReset()
    {
        _command.Model.Payments.Single(x => x.Amount == 100).SentForPayment.Should().BeFalse();
        _command.Model.Payments.Single(x => x.Amount == 200).SentForPayment.Should().BeFalse();
        _command.Model.Payments.Single(x => x.Amount == 300).SentForPayment.Should().BeTrue();
        _command.Model.Payments.Single(x => x.Amount == 400).SentForPayment.Should().BeTrue();
    }

}