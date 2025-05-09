using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenCalculatePayments
{
    private Fixture _fixture;
    private Domain.Apprenticeship.Apprenticeship _sut;
    private AcademicYears _academicYears;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new EarningsGeneratedEventCustomization());
        var earningsGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();
        _sut = new Domain.Apprenticeship.Apprenticeship(earningsGeneratedEvent);
        _sut.ClearEarnings();
        _academicYears = TestHelper.CreateAcademicYears(DateTime.Now);
    }

    [Test]
    public void WhenAllEarningsInTheFutureThenPaymentsMatchEarnings()
    {
        var now = DateTime.Now;

        var earnings = new List<Earning>
        {
            new (_sut.ApprenticeshipKey, now.ToAcademicYear(), now.ToDeliveryPeriod(), _fixture.Create<decimal>(), (short)now.Year, (byte)now.Month,_fixture.Create<string>(), Guid.NewGuid(), InstalmentTypes.OnProgramme),
            new (_sut.ApprenticeshipKey, now.AddMonths(1).ToAcademicYear(), now.AddMonths(1).ToDeliveryPeriod(), _fixture.Create<decimal>(), (short)now.AddMonths(1).Year, (byte)now.AddMonths(1).Month, _fixture.Create<string>(), Guid.NewGuid(), InstalmentTypes.OnProgramme),
            new (_sut.ApprenticeshipKey, now.AddMonths(2).ToAcademicYear(), now.AddMonths(2).ToDeliveryPeriod(), _fixture.Create<decimal>(), (short)now.AddMonths(2).Year, (byte)now.AddMonths(2).Month, _fixture.Create<string>(), Guid.NewGuid(), InstalmentTypes.OnProgramme)
        };
        foreach (var earning in earnings)
        {
            _sut.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth, _fixture.Create<string>(), earning.EarningsProfileId, InstalmentTypes.OnProgramme);
        }

        _sut.CalculatePayments(DateTime.Now, _academicYears);

        earnings.ForEach(earning => _sut.Payments.Should()
            .Contain(x => x.SentForPayment == false &&
                          x.Amount == earning.Amount &&
                          x.AcademicYear == earning.AcademicYear &&
                          x.DeliveryPeriod == earning.DeliveryPeriod &&
                          x.EarningsProfileId == earning.EarningsProfileId));
    }

    [Test]
    public void CollectionPeriodShouldBeDeliveryPeriod()
    {
        _sut.AddEarning(2324, 1, _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(1).Year, (byte)DateTime.Now.AddMonths(1).Month,_fixture.Create<string>(), Guid.NewGuid(), InstalmentTypes.OnProgramme);

        _sut.CalculatePayments(DateTime.Now, _academicYears);

        _sut.Payments.Count.Should().Be(1);
        _sut.Payments.Single().CollectionYear.Should().Be(2324);
        _sut.Payments.Single().CollectionPeriod.Should().Be(1);
    }
}