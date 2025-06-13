using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Orchestrators;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.UnitTests.Orchestrators;

public class ReleasePaymentsForProviderOrchestratorTests
{
    private Mock<TaskOrchestrationContext> _orchestrationContextMock = null!;
    private Mock<ILogger<ReleasePaymentsForProviderOrchestrator>> _loggerMock = null!;
    private ReleasePaymentsForProviderInput _input = null!;
    private IEnumerable<Learner> _learnersInIlr = null!;
    private ReleasePaymentsForProviderOrchestrator _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<ReleasePaymentsForProviderOrchestrator>>();
        _orchestrationContextMock = new Mock<TaskOrchestrationContext>();
        var orchestrationInstanceId = "test-instance-id";

        var collectionDetails = new CollectionDetails(
            collectionPeriod: 12,
            collectionYear: 2025
        );

        _input = new ReleasePaymentsForProviderInput(collectionDetails, 12345678, orchestrationInstanceId);

        _learnersInIlr = new List<Learner>
        {
            new Learner(12345678, 12345, "LRN001"),
            new Learner(12345678, 67890, "LRN002")
        };

        var ilrInput = new GetLearnersInIlrSubmissionInput(
            ukprn: _input.Ukprn,
            academicYear: collectionDetails.CollectionYear,
            orchestrationInstanceId
        );

        _orchestrationContextMock
            .Setup(x => x.GetInput<ReleasePaymentsForProviderInput>())
            .Returns(_input);

        _orchestrationContextMock
            .Setup(x => x.CallActivityAsync<IEnumerable<Learner>>(nameof(GetLearnersInIlrSubmission), It.Is<GetLearnersInIlrSubmissionInput>(i => i.Ukprn == ilrInput.Ukprn && i.AcademicYear == ilrInput.AcademicYear), null))
            .ReturnsAsync(_learnersInIlr);

        _orchestrationContextMock
            .Setup(x => x.CallSubOrchestratorAsync(It.IsAny<TaskName>(), It.IsAny<object>(), null))
            .Returns(Task.CompletedTask);

        _sut = new ReleasePaymentsForProviderOrchestrator(_loggerMock.Object);
    }

    [Test]
    public async Task RunOrchestrator_ProcessesLearnersCorrectly()
    {
        await _sut.RunOrchestrator(_orchestrationContextMock.Object);

        _orchestrationContextMock.Verify(x => x.CallActivityAsync<IEnumerable<Learner>>(
                nameof(GetLearnersInIlrSubmission),
                It.Is<GetLearnersInIlrSubmissionInput>(input =>
                    input.Ukprn == _input.Ukprn &&
                    input.AcademicYear == _input.CollectionDetails.CollectionYear), null),
            Times.Once);

        _orchestrationContextMock.Verify(x => x.CallSubOrchestratorAsync(
                It.Is<TaskName>(name => name.Name == nameof(ReleasePaymentsForLearnerOrchestrator)),
                It.IsAny<ReleasePaymentsForLearnerInput>(), null),
            Times.Exactly(_learnersInIlr.Count()));
    }

    [Test]
    public async Task RunOrchestrator_SetsCustomStatuses()
    {
        await _sut.RunOrchestrator(_orchestrationContextMock.Object);

        _orchestrationContextMock.Verify(x => x.SetCustomStatus("GettingIlrSubmissions"), Times.Once);
        _orchestrationContextMock.Verify(x => x.SetCustomStatus("ReleasingPaymentsForLearners"), Times.Once);
    }
}