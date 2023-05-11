using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests;

public class FinalisedOnProgammeLearningPaymentEventBuilder_BuildTests
{
    private FinalisedOnProgammeLearningPaymentEventBuilder _sut;
    private FinalisedOnProgammeLearningPaymentEvent _result;
    private Fixture _fixture;
    private PaymentEntityModel _paymentEntityModel;
    private Guid _apprenticeshipKey;

    [SetUp]
    public void SetUp()
    {
        _sut = new FinalisedOnProgammeLearningPaymentEventBuilder();
        _fixture = new Fixture();

        _paymentEntityModel = _fixture.Create<PaymentEntityModel>();
        _apprenticeshipKey = Guid.NewGuid();

        _result = _sut.Build(_paymentEntityModel, _apprenticeshipKey);
    }

    [Test]
    public void ShouldPopulateTheApprenticeshipKeyCorrectly()
    {
        _result.ApprenticeshipKey.Should().Be(_apprenticeshipKey);
    }

    [Test]
    public void ShouldPopulateTheCollectionYearCorrectly()
    {
        _result.CollectionYear.Should().Be(_paymentEntityModel.PaymentYear);
    }

    [Test]
    public void ShouldPopulateTheCollectionMonthCorrectly()
    {
        _result.CollectionMonth.Should().Be(_paymentEntityModel.PaymentPeriod);
    }

    [Test]
    public void ShouldPopulateTheAmountCorrectly()
    {
        _result.Amount.Should().Be(_paymentEntityModel.Amount);
    }
}