using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenGetDuePayments
{
    private Fixture _fixture;
    private Domain.Apprenticeship.Apprenticeship _sut;
    private ReadOnlyCollection<Payment> _duePayments;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new EarningsGeneratedEventCustomization());
        var currentMonthlyLearningAmount = _fixture.Create<decimal>();

        var earningGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();
        earningGeneratedEvent.DeliveryPeriods = new List<DeliveryPeriod>
        {
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2122).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 12).With(x => x.CalendarMonth, 7)
                .With(x => x.LearningAmount, currentMonthlyLearningAmount).Create(),
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 1).With(x => x.CalendarMonth, 8)
                .With(x => x.LearningAmount, currentMonthlyLearningAmount).Create(),
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 2).With(x => x.CalendarMonth, 9)
                .With(x => x.LearningAmount, currentMonthlyLearningAmount).Create(),
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 3).With(x => x.CalendarMonth, 10)
                .With(x => x.LearningAmount, currentMonthlyLearningAmount).Create(),
        };
        _sut = new Domain.Apprenticeship.Apprenticeship(earningGeneratedEvent);
        _sut.CalculatePayments(new DateTime(2022, 8, 1));

        _duePayments = _sut.DuePayments(2223, 2);
    }

    [Test]
    public void OnlyCurrentAcademicYearPaymentsAreReturned()
    {
        _duePayments.Count.Should().Be(2);
        _duePayments.Should().AllSatisfy(x => x.AcademicYear.Should().Be(2223));
    }

    [Test]
    public void OnlyPaymentsOnOrBeforeTheCollectionPeriodAreReturned()
    {
        _duePayments.Count.Should().Be(2);
        _duePayments.Should().AllSatisfy(x => x.DeliveryPeriod.Should().BeLessThanOrEqualTo(2));
    }
}