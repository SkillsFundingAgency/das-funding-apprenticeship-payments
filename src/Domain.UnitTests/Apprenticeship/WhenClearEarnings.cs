using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenClearEarnings
{
    private Fixture _fixture;
    private Domain.Apprenticeship.Apprenticeship _sut;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new EarningsGeneratedEventCustomization());
        var earningGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();
        var academicYears = TestHelper.CreateAcademicYears(DateTime.Now.AddYears(-1));
        _sut = new Domain.Apprenticeship.Apprenticeship(earningGeneratedEvent);
        _sut.CalculatePayments(DateTime.Now, academicYears);
    }

    [Test]
    public void AllExistingEarningsShouldBeCleared()
    {
        _sut.ClearEarnings();
        _sut.Earnings.Should().BeEmpty();
    }
}