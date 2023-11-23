using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Apprenticeship;

[TestFixture]
public class WhenRecalculatePayments
{
    private Fixture _fixture;
    private List<Earning> _newEarnings;
    private List<Payment> _existingPayments;
    private decimal _currentMonthlyLearningAmount;
    private decimal _newMonthlyLearningAmount;
    private Guid _previousEarningsProfileId;
    private Guid _newEarningsProfileId;
    private Domain.Apprenticeship.Apprenticeship _sut;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _currentMonthlyLearningAmount = _fixture.Create<decimal>();
        _newMonthlyLearningAmount = _fixture.Create<decimal>();
        _previousEarningsProfileId = Guid.NewGuid();
        _newEarningsProfileId = Guid.NewGuid();

        //Expectation:
        //Delivery Period 1 - diff payment calculated
        //Delivery Period 2 - unpaid payment removed and new one calculated for new learning amount
        //Delivery Period 3 - no existing payments, new one calculated for new learning amount
        _newEarnings = new List<Earning>
        {
            new Earning(2223, 1, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId),
            new Earning(2223, 2, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId),
            new Earning(2223, 3, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId)
        };

        _existingPayments = new List<Payment>()
        {
            new Payment(2223, 1, _currentMonthlyLearningAmount, 2022, 8, "payment 1 paid", _previousEarningsProfileId){ SentForPayment = true },
            new Payment(2223, 2, _currentMonthlyLearningAmount, 2022, 8, "payment 2 unpaid", _previousEarningsProfileId)
        };

        _sut = new Domain.Apprenticeship.Apprenticeship(Guid.NewGuid(), new List<Earning>(), _existingPayments);

        foreach (var newEarning in _newEarnings)
        {
            _sut.AddEarning(newEarning.AcademicYear, newEarning.DeliveryPeriod, newEarning.Amount, newEarning.CollectionYear, newEarning.CollectionMonth, newEarning.FundingLineType, newEarning.EarningsProfileId);
        }

        _sut.RecalculatePayments(DateTime.Now);
    }

    [Test]
    public void ExistingUnpaidPaymentsShouldBeRemoved()
    {
        _sut.Payments.Should().NotContain(p => p.FundingLineType == "payment 2 unpaid");
    }

    [Test]
    public void ExistingPaidPaymentsShouldBeNotBeRemoved()
    {
        _sut.Payments.Should().Contain(p => p.FundingLineType == "payment 1 paid" && p.EarningsProfileId == _previousEarningsProfileId);
    }

    [Test]
    public void NewPaymentsToMakeUpExistingPaidPaymentsAmountsShouldBeAdded()
    {
        _sut.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 1 && p.Amount == _newMonthlyLearningAmount - _currentMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void NewPaymentsForPreviouslyRemovedUnpaidPaymentPeriodsShouldBeAdded()
    {
        _sut.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 2 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void NewPaymentsForPreviouslyUncalculatedPaymentPeriodsShouldBeAdded()
    {
        _sut.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 3 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }
}