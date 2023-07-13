using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests
{
    public class ApprenticeshipEntity_HandleTests
    {
        private ApprenticeshipEntity _sut = null!;
        private EarningsGeneratedEvent _earningsGeneratedEvent = null!;
        private Mock<ICalculateApprenticeshipPaymentsCommandHandler> _calculateApprenticeshipPaymentsCommandHandler = null!;
        private Mock<IDomainEventDispatcher> _domainEventDispatcher = null!;
        private Fixture _fixture = null!;
        private Apprenticeship _apprenticeship = null!;
        private IEnumerable<EarningEntityModel> _expectedEarnings = null!;

        [SetUp]
        public async Task SetUp()
        {
            _fixture = new Fixture();

            _earningsGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();
            _earningsGeneratedEvent.DeliveryPeriods.ForEach(d => d.AcademicYear = AcademicYearHelper.GetRandomValidAcademicYear());
            _earningsGeneratedEvent.Uln = _fixture.Create<long>().ToString();

            _expectedEarnings = _earningsGeneratedEvent.DeliveryPeriods.Select(y =>
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
            _sut = new ApprenticeshipEntity(_calculateApprenticeshipPaymentsCommandHandler.Object, _domainEventDispatcher.Object, Mock.Of<IProcessUnfundedPaymentsCommandHandler>(), Mock.Of<ILogger<ApprenticeshipEntity>>());

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
        public void ShouldMapStartDateToEntity()
        {
            _sut.Model.StartDate.Should().Be(_earningsGeneratedEvent.StartDate);
        }

        [Test]
        public void ShouldMapUkprnToEntity()
        {
            _sut.Model.Ukprn.Should().Be(_earningsGeneratedEvent.ProviderId);
        }

        [Test]
        public void ShouldMapUlnToEntity()
        {
            _sut.Model.Uln.Should().Be(long.Parse(_earningsGeneratedEvent.Uln));
        }

        [Test]
        public void ShouldMapPlannedEndDateToEntity()
        {
            _sut.Model.PlannedEndDate.Should().Be(_earningsGeneratedEvent.ActualEndDate); //todo is this right?
        }

        [Test]
        public void ShouldMapCourseCodeToEntity()
        {
            _sut.Model.CourseCode.Should().Be(_earningsGeneratedEvent.TrainingCode);
        }

        [Test]
        public void ShouldMapFundingEmployerAccountIdToEntity()
        {
            _sut.Model.FundingEmployerAccountId.Should().Be(_earningsGeneratedEvent.EmployerAccountId);
        }
    }
}