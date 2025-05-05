using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Queries.UnitTests;

[TestFixture]
public class GetPaymentsDueQueryHandlerTests
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public async Task WhenGetThenPaymentsAreReturned()
    {
        var repository = new Mock<IApprenticeshipRepository>();
        var sut = new GetDuePaymentsQueryHandler(repository.Object);

        var query = _fixture.Create<GetDuePaymentsQuery>();
        var expectedPayments = _fixture.CreateMany<Payment>();

        var apprenticeship = new Mock<IApprenticeship>();
        apprenticeship.Setup(x => x.DuePayments(query.CollectionYear, query.CollectionPeriod, query.PaymentType)).Returns(expectedPayments.ToList().AsReadOnly);

        repository.Setup(x => x.Get(query.ApprenticeshipKey)).ReturnsAsync(apprenticeship.Object);

        var actualPayments = await sut.Get(query);

        actualPayments.Payments.Should().AllSatisfy(x => expectedPayments.Should().Contain(y => y.Key == x.Key));
    }
}