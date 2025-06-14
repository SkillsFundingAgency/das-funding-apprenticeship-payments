﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenRecalculatePayments
{
    private Fixture _fixture;
    private List<Earning> _newEarnings;
    private decimal _currentMonthlyLearningAmount;
    private decimal _newMonthlyLearningAmount;
    private Guid _newEarningsProfileId;
    private Domain.Apprenticeship.Apprenticeship _sut;
    private List<Payment> _originalPayments;
    private DateTime _previousAcademicYearHardClose;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Customize(new EarningsGeneratedEventCustomization());
        _currentMonthlyLearningAmount = _fixture.Create<decimal>();
        _newMonthlyLearningAmount = _fixture.Create<decimal>();
        _newEarningsProfileId = Guid.NewGuid();
        _previousAcademicYearHardClose = new DateTime(2021,1,1);
        var academicYear = TestHelper.CreateAcademicYears(new DateTime(2022, 1, 1));

        var earningGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();
        earningGeneratedEvent.DeliveryPeriods = new List<DeliveryPeriod>
        {
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 1).With(x => x.CalendarMonth, 10)
                .With(x => x.LearningAmount, _currentMonthlyLearningAmount)
                .With(x => x.InstalmentType, InstalmentTypes.OnProgramme).Create(),
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 2).With(x => x.CalendarMonth, 11)
                .With(x => x.LearningAmount, _currentMonthlyLearningAmount)
                .With(x => x.InstalmentType, InstalmentTypes.OnProgramme).Create(),
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 3).With(x => x.CalendarMonth, 12)
                .With(x => x.LearningAmount, _currentMonthlyLearningAmount)
                .With(x => x.InstalmentType, InstalmentTypes.OnProgramme).Create(),
            _fixture.Build<DeliveryPeriod>().With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022)
                .With(x => x.Period, 5).With(x => x.CalendarMonth, 12)
                .With(x => x.LearningAmount, _currentMonthlyLearningAmount)
                .With(x => x.InstalmentType, InstalmentTypes.MathsAndEnglish).Create(),
        };
        _sut = new Domain.Apprenticeship.Apprenticeship(earningGeneratedEvent);
        _sut.CalculatePayments(DateTime.Now, academicYear);
        _sut.Payments.First().MarkAsSent(2223, 2);
        _sut.Payments.ElementAt(1).MarkAsSent(2223, 3);

        _originalPayments = new List<Payment>(_sut.Payments);

        _sut.ClearEarnings();

        //Expectation:
        //Delivery Period 1 - existing payment matches earning, no payment to be generated (e.g. if there was a price change after this period)
        //Delivery Period 2 - diff payment calculated
        //Delivery Period 3 - unpaid payment removed and new one calculated for new learning amount
        //Delivery Period 4 - no existing payments, new one calculated for new learning amount
        //Delivery Period 5 - multiple payments of the same type
        _newEarnings = new List<Earning>
        {
            new Earning(_sut.ApprenticeshipKey, 2223, 1, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId, InstalmentTypes.OnProgramme),
            new Earning(_sut.ApprenticeshipKey, 2223, 2, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId, InstalmentTypes.OnProgramme),
            new Earning(_sut.ApprenticeshipKey, 2223, 3, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId, InstalmentTypes.OnProgramme),
            new Earning(_sut.ApprenticeshipKey, 2223, 4, _newMonthlyLearningAmount, 2022, 11, _fixture.Create<string>(), _newEarningsProfileId, InstalmentTypes.OnProgramme),
            new Earning(_sut.ApprenticeshipKey, 2223, 5, _newMonthlyLearningAmount, 2022, 12, _fixture.Create<string>(), _newEarningsProfileId, InstalmentTypes.MathsAndEnglish),
            new Earning(_sut.ApprenticeshipKey, 2223, 5, _newMonthlyLearningAmount, 2022, 12, _fixture.Create<string>(), _newEarningsProfileId, InstalmentTypes.MathsAndEnglish)
        };

        foreach (var newEarning in _newEarnings)
        {
            _sut.AddEarning(newEarning.AcademicYear, newEarning.DeliveryPeriod, newEarning.Amount, newEarning.CollectionYear, newEarning.CollectionMonth, newEarning.FundingLineType, newEarning.EarningsProfileId, newEarning.InstalmentType);
        }

        _sut.RecalculatePayments(DateTime.Now, academicYear);
    }

    [Test]
    public void ExistingUnpaidPaymentsShouldBeRemoved()
    {
        _sut.Payments.Should().NotContain(_originalPayments.Where(x => !x.SentForPayment));
    }

    [Test]
    public void ExistingPaidPaymentsShouldBeNotBeRemoved()
    {
        _sut.Payments.Should().Contain(_originalPayments.Where(x => x.SentForPayment));
    }

    [Test]
    public void NewPaymentsToMakeUpExistingPaidPaymentsAmountsShouldBeAdded()
    {
        _sut.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 2 && p.Amount == _newMonthlyLearningAmount - _currentMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void NewPaymentsForPreviouslyRemovedUnpaidPaymentPeriodsShouldBeAdded()
    {
        _sut.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 3 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void NewPaymentsForPreviouslyUncalculatedPaymentPeriodsShouldBeAdded()
    {
        _sut.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 4 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void PaymentsForPreviouslyPaidPaymentPeriodsWithNoChangeShouldNotBeAdded()
    {
        _sut.Payments.Should().NotContain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 1 && p.Amount == 0);
    }

    [Test]
    public void MultiplePaymentsOfTheSameTypeShouldBeAdded()
    {
        _sut.Payments.Where(p => p.AcademicYear == 2223 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId && p.DeliveryPeriod == 5).Should().HaveCount(2);
    }
}