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
    private ApprenticeshipEntityModel _apprenticeshipEntityModel;

    [SetUp]
    public void SetUp()
    {
        _sut = new FinalisedOnProgammeLearningPaymentEventBuilder();
        _fixture = new Fixture();

        _paymentEntityModel = _fixture.Create<PaymentEntityModel>();
        _apprenticeshipEntityModel = _fixture.Create<ApprenticeshipEntityModel>();

        _result = _sut.Build(_paymentEntityModel, _apprenticeshipEntityModel);
    }

    [Test]
    public void ShouldPopulateTheApprenticeshipKeyCorrectly()
    {
        _result.ApprenticeshipKey.Should().Be(_apprenticeshipEntityModel.ApprenticeshipKey);
    }

    [Test]
    public void ShouldPopulateTheCollectionYearCorrectly()
    {
        _result.CollectionYear.Should().Be(_paymentEntityModel.CollectionYear);
    }

    [Test]
    public void ShouldPopulateTheCollectionMonthCorrectly()
    {
        _result.CollectionMonth.Should().Be(_paymentEntityModel.CollectionPeriod);
    }

    [Test]
    public void ShouldPopulateTheAmountCorrectly()
    {
        _result.Amount.Should().Be(_paymentEntityModel.Amount);
    }

    [Test]
    public void ShouldPopulateTheUlnCorrectly()
    {
        _result.ApprenticeshipEarning.Uln.Should().Be(_apprenticeshipEntityModel.Uln);
    }

    [Test]
    public void ShouldPopulateTheStartDateCorrectly()
    {
        _result.ApprenticeshipEarning.StartDate.Should().Be(_apprenticeshipEntityModel.StartDate);
    }

    [Test]
    public void ShouldPopulateThePlannedEndDateCorrectly()
    {
        _result.ApprenticeshipEarning.PlannedEndDate.Should().Be(_apprenticeshipEntityModel.PlannedEndDate);
    }

    [Test]
    public void ShouldPopulateTheUkprnCorrectly()
    {
        _result.ApprenticeshipEarning.ProviderIdentifier.Should().Be(_apprenticeshipEntityModel.Ukprn);
    }
}