using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

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
            var earnings = new List<Earning>()
            {
                new (AcademicYearHelper.GetRandomValidAcademicYear(), _fixture.Create<byte>(), _fixture.Create<decimal>()),
                new (AcademicYearHelper.GetRandomValidAcademicYear(), _fixture.Create<byte>(), _fixture.Create<decimal>()),
                new (AcademicYearHelper.GetRandomValidAcademicYear(), _fixture.Create<byte>(), _fixture.Create<decimal>())
            };
            foreach (var earning in earnings)
            {
                _sut.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount);
            }

            _sut.CalculatePayments();

            _sut.Payments.Should().BeEquivalentTo(earnings,
                opts => opts.ExcludingMissingMembers());
        }

        [TestCase(2223,1, 2223, 2)]
        [TestCase(2223, 12, 2324, 1)]
        public void PaymentPeriodShouldBeOneMonthAfterDeliveryPeriod(short earningAcademicYear, byte deliveryPeriod, short paymentAcademicYear, byte paymentPeriod)
        {
            _sut.AddEarning(earningAcademicYear, deliveryPeriod, _fixture.Create<decimal>());

            _sut.CalculatePayments();

            _sut.Payments.Count.Should().Be(1);
            _sut.Payments.Single().PaymentYear.Should().Be(paymentAcademicYear);
            _sut.Payments.Single().PaymentPeriod.Should().Be(paymentPeriod);
        }
    }
}
