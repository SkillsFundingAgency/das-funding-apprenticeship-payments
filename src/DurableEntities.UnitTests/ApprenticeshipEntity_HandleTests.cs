using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

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
            _earningsGeneratedEvent.FundingPeriods.ForEach(f => f.DeliveryPeriods.ForEach(d => d.AcademicYear = AcademicYearHelper.GetRandomValidAcademicYear()));

            _expectedEarnings = _earningsGeneratedEvent.FundingPeriods.SelectMany(x => x.DeliveryPeriods).Select(y =>
                new EarningEntityModel
                {
                    DeliveryPeriod = y.Period,
                    AcademicYear = y.AcademicYear,
                    Amount = y.LearningAmount,
                    CollectionMonth = y.CalendarMonth,
                    CollectionYear = y.CalenderYear
                });

            _apprenticeship = _fixture.Create<Apprenticeship>();

            _calculateApprenticeshipPaymentsCommandHandler = new Mock<ICalculateApprenticeshipPaymentsCommandHandler>();
            _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
            _sut = new ApprenticeshipEntity(_calculateApprenticeshipPaymentsCommandHandler.Object, _domainEventDispatcher.Object, Mock.Of<IProcessUnfundedPaymentsCommandHandler>());

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