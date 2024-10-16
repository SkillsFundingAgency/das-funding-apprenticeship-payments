using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipKey;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Queries.UnitTests
{
    [TestFixture]
    public class GetApprenticeshipKeyQueryHandlerTests
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async Task WhenGetThenApprenticeshipKeyIsReturned()
        {
            var repository = new Mock<IApprenticeshipQueryRepository>();
            var sut = new GetApprenticeshipKeyQueryHandler(repository.Object);

            var query = _fixture.Create<GetApprenticeshipKeyQuery>();

            var expectedKey = _fixture.Create<Guid>();
            repository.Setup(x => x.GetApprenticeshipKey(query.Ukprn, query.Uln)).ReturnsAsync(expectedKey);

            var actualProviders = await sut.Get(query);

            actualProviders.ApprenticeshipKey.Should().Be(expectedKey);
        }
    }
}
