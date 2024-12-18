using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture;
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
        _fixture.Customize(new EarningsGeneratedEventCustomization());

        _apprenticeship = _fixture.Create<Domain.Apprenticeship.Apprenticeship>();
        _apprenticeship.CalculatePayments(DateTime.Now);

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
        _result.Payments.Should().BeEquivalentTo(_apprenticeship.Payments, opts => opts.ExcludingMissingMembers());
    }
}