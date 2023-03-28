using System;
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
            var earnings = _fixture.CreateMany<Earning>();
            foreach (var earning in earnings)
            {
                _sut.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount);
            }

            _sut.CalculatePayments();

            _sut.Payments.Should().BeEquivalentTo(earnings, opts => opts.ExcludingMissingMembers());
        }
    }
}
