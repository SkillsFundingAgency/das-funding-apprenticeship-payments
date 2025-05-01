using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.UnitTests;

[TestFixture]
public class EarningsRecalculatedDeliveryPeriodExtensionsTests
{
    [Test]
    public void ToEarnings_ShouldConvertDeliveryPeriodsToEarnings()
    {
        // Arrange
        var fixture = new Fixture();
        var apprenticeshipKey = Guid.NewGuid();
        var earningsProfileId = Guid.NewGuid();
        var deliveryPeriods = fixture.Create<List<DeliveryPeriod>>();

        // Act
        var earnings = deliveryPeriods.ToEarnings(apprenticeshipKey, earningsProfileId);

        // Assert
        Assert.NotNull(earnings);
        Assert.AreEqual(deliveryPeriods.Count, earnings.Count);

        for (int i = 0; i < deliveryPeriods.Count; i++)
        {
            Assert.AreEqual(apprenticeshipKey, earnings[i].ApprenticeshipKey);
            Assert.AreEqual(deliveryPeriods[i].AcademicYear, earnings[i].AcademicYear);
            Assert.AreEqual(deliveryPeriods[i].Period, earnings[i].DeliveryPeriod);
            Assert.AreEqual(deliveryPeriods[i].LearningAmount, earnings[i].Amount);
            Assert.AreEqual(deliveryPeriods[i].CalenderYear, earnings[i].CollectionYear);
            Assert.AreEqual(deliveryPeriods[i].CalendarMonth, earnings[i].CollectionMonth);
            Assert.AreEqual(deliveryPeriods[i].FundingLineType, earnings[i].FundingLineType);
            Assert.AreEqual(earningsProfileId, earnings[i].EarningsProfileId);
            Assert.AreEqual(deliveryPeriods[i].InstalmentType, earnings[i].InstalmentType);
        }
    }
}