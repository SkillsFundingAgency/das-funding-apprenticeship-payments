using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests
{
    public class ApprenticeshipEntity_HandleTests
    {
        private ApprenticeshipEntity _sut;
        private EarningsGeneratedEvent _earningsGeneratedEvent;
        private Mock<ICalculateApprenticeshipPaymentsCommandHandler> _calculateApprenticeshipPaymentsCommandHandler;
        private Mock<IDomainEventDispatcher> _domainEventDispatcher;
        private Fixture _fixture;
        private Apprenticeship _apprenticeship;
        private IEnumerable<EarningEntityModel> _expectedEarnings;

        [SetUp]
        public async Task SetUp()
        {
            _fixture = new Fixture();

            _earningsGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();

            _expectedEarnings = _earningsGeneratedEvent.FundingPeriods.SelectMany(x => x.DeliveryPeriods).Select(y =>
                new EarningEntityModel
                {
                    DeliveryPeriod = y.Period,
                    AcademicYear = y.AcademicYear,
                    Amount = y.LearningAmount
                });

            _apprenticeship = new Apprenticeship(Guid.NewGuid());
            foreach (var earning in _expectedEarnings)
            {
                _apprenticeship.AddEarning(earning.AcademicYear, earning.DeliveryPeriod, earning.Amount);
            }
            _apprenticeship.CalculatePayments();

            _calculateApprenticeshipPaymentsCommandHandler = new Mock<ICalculateApprenticeshipPaymentsCommandHandler>();
            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
            _sut = new ApprenticeshipEntity(_calculateApprenticeshipPaymentsCommandHandler.Object, _domainEventDispatcher.Object);

            _calculateApprenticeshipPaymentsCommandHandler.Setup(x => x.Calculate(It.IsAny<CalculateApprenticeshipPaymentsCommand>())).ReturnsAsync(_apprenticeship);

            await _sut.HandleEarningsGeneratedEvent(_earningsGeneratedEvent);
        }

        [Test]
        public void ShouldMapApprenticeshipKeyToEntity()
        {
            _sut.Model.ApprenticeshipKey.Should().Be(_earningsGeneratedEvent.ApprenticeshipKey);
        }

        [Test]
        public void ShouldMapEarningsToEntity()
        {
            _sut.Model.Earnings.Should().BeEquivalentTo(_expectedEarnings);
        }

        [Test]
        public void ShouldCallGenerateEarnings()
        {
            _calculateApprenticeshipPaymentsCommandHandler.Verify(x => x.Calculate(It.Is<CalculateApprenticeshipPaymentsCommand>(y => y.ApprenticeshipEntity == _sut.Model)));
        }

        [Test]
        public void ShouldMapCalculatedPaymentsToEntity()
        {
            _sut.Model.Payments.Should().BeEquivalentTo(_apprenticeship.Payments);
        }
    }
}