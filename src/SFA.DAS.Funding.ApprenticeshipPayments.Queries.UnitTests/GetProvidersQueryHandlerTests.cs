using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetProviders;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Queries.UnitTests;

[TestFixture]
public class GetProvidersQueryHandlerTests
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public async Task WhenGetThenProvidersAreReturned()
    {
        var repository = new Mock<IApprenticeshipQueryRepository>();
        var sut = new GetProvidersQueryHandler(repository.Object);

        var expectedProviders = _fixture.CreateMany<long>();
        repository.Setup(x => x.GetAllProviders()).ReturnsAsync(expectedProviders);

        var actualProviders = await sut.Get(new GetProvidersQuery());

        actualProviders.Providers.Should().AllSatisfy(x => expectedProviders.Should().Contain(x.Ukprn));
    }
}