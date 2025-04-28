using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Queries.UnitTests
{
    [TestFixture]
    public class GetApprenticeshipsWithDuePaymentsQueryHandlerTests
    {
        private Fixture _fixture;
        private GetApprenticeshipsWithDuePaymentsQueryHandler _handler;
        private Mock<IApprenticeshipQueryRepository> _repositoryMock;
        private List<Guid> _apprenticeships;
        private GetApprenticeshipsWithDuePaymentsQuery _query;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _query = _fixture.Create<GetApprenticeshipsWithDuePaymentsQuery>();
            _apprenticeships = _fixture.Create<List<Guid>>();

            _repositoryMock = new Mock<IApprenticeshipQueryRepository>();
            _repositoryMock
                .Setup(x => x.GetWithDuePayments(_query.CollectionYear, _query.CollectionPeriod))
                .ReturnsAsync(_apprenticeships);

            _handler = new GetApprenticeshipsWithDuePaymentsQueryHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handler_Returns_Expected_Result()
        {
            var result = await _handler.Get(_query);

            var expected = _apprenticeships.Select(x => new Apprenticeship(x)).ToList();

            result.Apprenticeships.Should().BeEquivalentTo(expected);
        }
    }
}