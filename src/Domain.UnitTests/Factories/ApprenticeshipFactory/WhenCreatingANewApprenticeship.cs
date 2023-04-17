using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Factories.ApprenticeshipFactory
{
    [TestFixture]
    public class WhenCreatingANewApprenticeship
    {
        private Fixture _fixture;
        private Domain.Factories.ApprenticeshipFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _factory = new Domain.Factories.ApprenticeshipFactory();
        }

        [Test]
        public void ThenTheApprenticeshipIsCreated()
        {
            var apprenticeshipEntityModel = _fixture.Create<ApprenticeshipEntityModel>();

            var apprenticeship = _factory.CreateNew(apprenticeshipEntityModel);

            apprenticeship.ApprenticeshipKey.Should().Be(apprenticeshipEntityModel.ApprenticeshipKey);
            apprenticeship.Earnings.Should().BeEquivalentTo(apprenticeshipEntityModel.Earnings);
        }
    }
}
