using System;
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
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.Year, (byte)DateTime.Now.Month),
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(1).Year, (byte)DateTime.Now.AddMonths(1).Month),
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(2).Year, (byte)DateTime.Now.AddMonths(2).Month)
            };
            foreach (var earning in earnings)
            {
                _sut.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth);
            }

            _sut.CalculatePayments();

            for (int i = 0; i < _sut.Payments.Count; i++)
            {
                _sut.Payments[i].DeliveryPeriod.Should().Be(earnings[i].DeliveryPeriod);
                _sut.Payments[i].AcademicYear.Should().Be(earnings[i].AcademicYear);
                _sut.Payments[i].Amount.Should().Be(earnings[i].Amount);
                _sut.Payments[i].PaymentYear.Should().Be(earnings[i].CollectionYear);
                _sut.Payments[i].PaymentPeriod.Should().Be(earnings[i].CollectionMonth);
            }
        }

        [Test]
        public void WhenEarningInAPreviousMonthThenPaymentAllocatedToCurrentMonth()
        {
            var earnings = new List<Earning>
            {
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(-2).Year, (byte)DateTime.Now.AddMonths(-2).Month),
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(-1).Year, (byte)DateTime.Now.AddMonths(-1).Month),
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.Year, (byte)DateTime.Now.Month),
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(1).Year, (byte)DateTime.Now.AddMonths(1).Month),
                new (_fixture.Create<short>(), _fixture.Create<byte>(), _fixture.Create<decimal>(), (short)DateTime.Now.AddMonths(2).Year, (byte)DateTime.Now.AddMonths(2).Month)
            };
            foreach (var earning in earnings)
            {
                _sut.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount, earning.CollectionYear, earning.CollectionMonth);
            }

            _sut.CalculatePayments();

            _sut.Payments[0].PaymentYear.Should().Be((short)DateTime.Now.Year);
            _sut.Payments[0].PaymentPeriod.Should().Be((byte)DateTime.Now.Month);
            _sut.Payments[1].PaymentYear.Should().Be((short)DateTime.Now.Year);
            _sut.Payments[1].PaymentPeriod.Should().Be((byte)DateTime.Now.Month);
            _sut.Payments[2].PaymentYear.Should().Be((short)DateTime.Now.Year);
            _sut.Payments[2].PaymentPeriod.Should().Be((byte)DateTime.Now.Month);
            _sut.Payments[3].PaymentYear.Should().Be(earnings[3].CollectionYear);
            _sut.Payments[3].PaymentPeriod.Should().Be(earnings[3].CollectionMonth);
            _sut.Payments[4].PaymentYear.Should().Be(earnings[4].CollectionYear);
            _sut.Payments[4].PaymentPeriod.Should().Be(earnings[4].CollectionMonth);
            for (int i = 0; i < _sut.Payments.Count; i++)
            {
                _sut.Payments[i].DeliveryPeriod.Should().Be(earnings[i].DeliveryPeriod);
                _sut.Payments[i].AcademicYear.Should().Be(earnings[i].AcademicYear);
                _sut.Payments[i].Amount.Should().Be(earnings[i].Amount);
            }
        }

        [TestCase(2223, 1, 2223, 2, 2223, 2)]
        [TestCase(2223, 12, 2324, 1, 2324, 1)]
        public void PaymentPeriodShouldBeOneMonthAfterDeliveryPeriod(short earningAcademicYear, byte deliveryPeriod, short paymentAcademicYear, byte paymentPeriod, short collectionYear, byte collectionMonth)
        {
            _sut.AddEarning(earningAcademicYear, deliveryPeriod, _fixture.Create<decimal>(), collectionYear, collectionMonth);

            _sut.CalculatePayments();

            _sut.Payments.Count.Should().Be(1);
            _sut.Payments.Single().PaymentYear.Should().Be(paymentAcademicYear);
            _sut.Payments.Single().PaymentPeriod.Should().Be(paymentPeriod);
        }
    }
}