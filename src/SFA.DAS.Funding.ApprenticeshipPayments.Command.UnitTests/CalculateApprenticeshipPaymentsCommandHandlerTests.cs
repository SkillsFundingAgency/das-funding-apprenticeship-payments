using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Requests;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api.Responses;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Api;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using System.Net;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class CalculateApprenticeshipPaymentsCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IApprenticeshipRepository> _apprenticeshipRepositoryMock;
    private readonly Mock<IDasServiceBusEndpoint> _busEndpointMock;
    private readonly Mock<IOuterApiClient> _apiClient = null!;
    private readonly CalculateApprenticeshipPaymentsCommandHandler _handler;

    public CalculateApprenticeshipPaymentsCommandHandlerTests()
    {
        _fixture = new Fixture();
        _apprenticeshipRepositoryMock = new Mock<IApprenticeshipRepository>();
        _busEndpointMock = new Mock<IDasServiceBusEndpoint>();
        Mock<IPaymentsGeneratedEventBuilder> eventBuilderMock = new();
        Mock<ISystemClockService> systemClockServiceMock = new();
        Mock<ILogger<CalculateApprenticeshipPaymentsCommandHandler>> loggerMock = new();

        _apiClient = new Mock<IOuterApiClient>();
        _apiClient.Setup(x => x.Get<GetAcademicYearsResponse>(It.IsAny<GetAcademicYearsRequest>()))
            .ReturnsAsync(
                new ApiResponse<GetAcademicYearsResponse>(
                    AcademicYearHelper.GetMockedAcademicYear<GetAcademicYearsResponse>(DateTime.Now.AddYears(-1)) , HttpStatusCode.OK, ""));


        systemClockServiceMock
            .Setup(clock => clock.Now)
            .Returns(DateTime.UtcNow);

        var paymentsEvent = _fixture.Create<PaymentsGeneratedEvent>();

        eventBuilderMock
            .Setup(builder => builder.Build(It.IsAny<Apprenticeship>()))
            .Returns(paymentsEvent);

        _handler = new CalculateApprenticeshipPaymentsCommandHandler(
            _apprenticeshipRepositoryMock.Object,
            _busEndpointMock.Object,
            eventBuilderMock.Object,
            systemClockServiceMock.Object,
            _apiClient.Object,
            loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_Process_Command_And_Publish_Event()
    {
        // Arrange
        var earningsGeneratedEvent = _fixture
            .Build<EarningsGeneratedEvent>()
            .With(e => e.Uln, "1774245662")
            .With(e=> e.DeliveryPeriods, new List<DeliveryPeriod>
            {
                new DeliveryPeriod(
                    12,
                    (short)DateTime.Now.Year,
                    12,
                    DateTime.Now.ToAcademicYear(),
                    200,
                    "Test",
                    "Test")
            })
            .Create();

        var command = new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent);

        // Act
        await _handler.Handle(command);

        // Assert
        _apprenticeshipRepositoryMock.Verify(repo => repo.Add(It.IsAny<Apprenticeship>()), Times.Once);
        _busEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<object>()), Times.Once);
    }

    [Test]
    public async Task Handle_Should_Do_Nothing_If_Apprenticeship_Already_Exists()
    {
        // Arrange
        var earningsGeneratedEvent = _fixture
            .Build<EarningsGeneratedEvent>()
            .With(e => e.Uln, "1774245662")
            .Create();

        var command = new CalculateApprenticeshipPaymentsCommand(earningsGeneratedEvent);

        _apprenticeshipRepositoryMock.Setup(x => x.Exists(It.Is<Guid>(key => key == command.EarningsGeneratedEvent.ApprenticeshipKey)))
            .ReturnsAsync(true);
        
        // Act
        await _handler.Handle(command);

        // Assert
        _apprenticeshipRepositoryMock.Verify(repo => repo.Add(It.IsAny<Apprenticeship>()), Times.Never);
        _busEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<object>()), Times.Never);
    }
}