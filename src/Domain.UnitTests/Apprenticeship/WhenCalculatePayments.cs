﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenCalculatePayments
    {
        private Fixture _fixture;
        private Domain.Apprenticeship.Apprenticeship _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _sut = new Domain.Apprenticeship.Apprenticeship(Guid.NewGuid());
        }

        [Test]
        public void WhenAllEarningsInTheFutureThenPaymentsMatchEarnings()
        {
            var earnings = new List<Earning>
            {
                new ((short)DateTime.Now.Year, _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.Year, (byte)DateTime.Now.Month),
                new ((short)DateTime.Now.AddMonths(1).Year, _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(1).Year, (byte)DateTime.Now.AddMonths(1).Month),
                new ((short)DateTime.Now.AddMonths(2).Year, _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(2).Year, (byte)DateTime.Now.AddMonths(2).Month)
            };
            foreach (var earning in earnings)
            {
                _sut.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth);
            }

            _sut.CalculatePayments(DateTime.Now);

            earnings.ForEach(earning => _sut.Payments.Should().Contain(x => x.SentForPayment == false && x.Amount == earning.Amount && x.AcademicYear == earning.AcademicYear && x.DeliveryPeriod == earning.DeliveryPeriod));
        }

        [Test]
        public void CollectionPeriodShouldBeDeliveryPeriodIfInTheFuture()
        {
            _sut.AddEarning(2324, 1, _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(1).Year, (byte)DateTime.Now.AddMonths(1).Month);

            _sut.CalculatePayments(DateTime.Now);

            _sut.Payments.Count.Should().Be(1);
            _sut.Payments.Single().CollectionYear.Should().Be(2324);
            _sut.Payments.Single().CollectionPeriod.Should().Be(1);
        }

        [Test]
        public void CollectionPeriodShouldBeCurrentPeriodIfInThePast()
        {
            var now = new DateTime(2023, 6, 25);
            _sut.AddEarning(2223, 11, _fixture.Create<decimal>(), (short)now.AddMonths(-1).Year, (byte)now.AddMonths(-1).Month);

            _sut.CalculatePayments(now);

            _sut.Payments.Count.Should().Be(1);
            _sut.Payments.Single().CollectionYear.Should().Be(2223);
            _sut.Payments.Single().CollectionPeriod.Should().Be(11);
        }
    }
}