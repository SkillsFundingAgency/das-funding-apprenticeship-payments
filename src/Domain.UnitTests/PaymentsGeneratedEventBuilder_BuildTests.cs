using System;
using System.Collections.Generic;
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
        var earnings = new List<Earning>
        {
            new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.Year, (byte)DateTime.Now.Month),
            new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(1).Year, (byte)DateTime.Now.AddMonths(1).Month),
            new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(2).Year, (byte)DateTime.Now.AddMonths(2).Month)
        };
        foreach (var earning in earnings)
        {
            _apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth);
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
    public void ShouldPopulateThePaymentsCorrectly()
    {
        _result.Payments.Should().BeEquivalentTo(_apprenticeship.Payments);
    }
}