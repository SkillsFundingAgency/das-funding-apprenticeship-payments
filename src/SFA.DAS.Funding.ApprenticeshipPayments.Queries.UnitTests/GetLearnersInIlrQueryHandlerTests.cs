using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetLearnersInILR;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Queries.UnitTests
{
    [TestFixture]
    public class GetLearnersInIlrQueryHandlerTests
    {
        private Fixture _fixture;
        private GetLearnersInILRQueryHandler _handler;
        private Mock<IOuterApiClient> _outerApiClient;
        private List<LearnerReferenceResponse> _response;
        private GetLearnersInILRQuery _query;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _query = _fixture.Create<GetLearnersInILRQuery>();
            _response = _fixture.Create<List<LearnerReferenceResponse>>();

            var expectedRequest = new GetLearnersInILRRequest(_query.Ukprn, _query.AcademicYear);
            _outerApiClient = new Mock<IOuterApiClient>();
            _outerApiClient.Setup(x => x.Get<List<LearnerReferenceResponse>>(It.Is<GetLearnersInILRRequest>(r => r.GetUrl == expectedRequest.GetUrl)))
                .ReturnsAsync(new ApiResponse<List<LearnerReferenceResponse>>(_response, HttpStatusCode.OK, ""));

            _handler = new GetLearnersInILRQueryHandler(_outerApiClient.Object,
                Mock.Of<ILogger<GetLearnersInILRQueryHandler>>());
        }

        [Test]
        public async Task Handler_Returns_Expected_Result()
        {
            var result = await _handler.Get(_query);

            var expected = _response.Select(x => new Learner(x.Uln, x.LearnerRefNumber)).ToList();

            result.Learners.Should().BeEquivalentTo(expected);
        }
    }
}