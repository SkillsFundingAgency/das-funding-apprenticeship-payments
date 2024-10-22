using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ApplyFreezeAndUnfreeze;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using System.Net;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class ApplyFreezeAndUnfreezeCommandHandler_ProcessUnfrozenTests
{
    private Mock<IApprenticeship> _apprenticeship = null!;
    private ApplyFreezeAndUnfreezeCommand _command = null!;
    private Fixture _fixture = null!;
    private byte _collectionPeriod;
    private short _collectionYear;
    private short _previousAcademicYear;
    private DateTime _hardCloseDate;
    private Mock<IApprenticeshipRepository> _repository = null!;
    private Mock<ISystemClockService> _systemClockService = null!;
    private Mock<IApprenticeshipsApiClient> _apiClient = null!;
    private ApplyFreezeAndUnfreezeCommandHandler _sut = null!;
    private DateTime _expectedCurrentDate;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new Fixture();
        _collectionPeriod = _fixture.Create<byte>();
        _collectionYear = 2425;
        _previousAcademicYear = 2324;
        _hardCloseDate = new DateTime(2025, 10, 15);
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeship.SetupGet(x => x.PaymentsFrozen).Returns(false);
        _command = new ApplyFreezeAndUnfreezeCommand(_fixture.Create<Guid>(), _collectionYear, _collectionPeriod);

        _repository = new Mock<IApprenticeshipRepository>();
        _repository.Setup(x => x.Get(_command.ApprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);
        _systemClockService = new Mock<ISystemClockService>();
        _expectedCurrentDate = DateTime.Now;
        _systemClockService.Setup(x => x.Now).Returns(_expectedCurrentDate);

        _apiClient = new Mock<IApprenticeshipsApiClient>();
        _apiClient.Setup(x => x.Get<GetAcademicYearsResponse>(It.Is<GetAcademicYearsRequest>(y =>
                y.GetUrl == $"CollectionCalendar/academicYear/{_expectedCurrentDate.ToString("yyyy-MM-dd HH:mm:ss")}")))
            .ReturnsAsync(
                new ApiResponse<GetAcademicYearsResponse>(new GetAcademicYearsResponse { AcademicYear = _collectionYear.ToString(), StartDate = _expectedCurrentDate.AddYears(-1) }, HttpStatusCode.OK, ""));

        _apiClient.Setup(x => x.Get<GetAcademicYearsResponse>(It.Is<GetAcademicYearsRequest>(y =>
                y.GetUrl == $"CollectionCalendar/academicYear/{_expectedCurrentDate.AddYears(-1).AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")}")))
            .ReturnsAsync(
                new ApiResponse<GetAcademicYearsResponse>(new GetAcademicYearsResponse { AcademicYear = _previousAcademicYear.ToString(), HardCloseDate = _hardCloseDate }, HttpStatusCode.OK, ""));

        _sut = new ApplyFreezeAndUnfreezeCommandHandler(_repository.Object, _systemClockService.Object, _apiClient.Object, Mock.Of<ILogger<ApplyFreezeAndUnfreezeCommandHandler>>());

        await _sut.Apply(_command);
    }

    [Test]
    public void ThenPreviouslyFrozenPaymentAreUnfrozen()
    {
        _apprenticeship.Verify(x => x.UnfreezeFrozenPayments(_collectionYear, _collectionPeriod, _collectionYear, _previousAcademicYear, _hardCloseDate, _expectedCurrentDate), Times.Once);
    }
}