using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

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
        _sut = new Domain.Apprenticeship.Apprenticeship(Guid.NewGuid(), _fixture.CreateMany<Earning>().ToList(), new List<Payment>());
    }

    [Test]
    public void AllExistingEarningsShouldBeCleared()
    {
        _sut.ClearEarnings();
        _sut.Earnings.Should().BeEmpty();
    }
}