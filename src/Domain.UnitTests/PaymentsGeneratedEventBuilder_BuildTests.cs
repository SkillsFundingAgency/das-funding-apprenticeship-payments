using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests;

public class PaymentsGeneratedEventBuilder_BuildTests
{
    private PaymentsGeneratedEventBuilder _sut;
    private PaymentsGeneratedEvent _result;
    private Fixture _fixture;
    private Domain.Apprenticeship.Apprenticeship _apprenticeship;

    [SetUp]
    public void SetUp()
    {
        _sut = new PaymentsGeneratedEventBuilder();
        _fixture = new Fixture();

        _apprenticeship = new Domain.Apprenticeship.Apprenticeship(Guid.NewGuid());
        var earnings = _fixture.CreateMany<Earning>();
        foreach (var earning in earnings)
        {
            _apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount);
        }
        _apprenticeship.CalculatePayments();

        _result = _sut.Build(_apprenticeship);
    }

    [Test]
    public void ShouldPopulateTheApprenticeshipKeyCorrectly()
    {
        _result.ApprenticeshipKey.Should().Be(_apprenticeship.ApprenticeshipKey);
    }

    [Test]
    public void ShouldPopulateThePaymentsCorrecly()
    {
        _result.Payments.Should().BeEquivalentTo(_apprenticeship.Payments);
    }
}