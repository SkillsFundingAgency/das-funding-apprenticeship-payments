using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class CalculateRequiredLevyAmountCommandHandler_ProcessTests
{
    private readonly Fixture _fixture = new();
    private Mock<IMessageSession> _messageSession = null!;
    private CalculateRequiredLevyAmountCommandHandler _sut = null!;
    private FinalisedOnProgammeLearningPaymentEvent _finalisedOnProgammeLearningPaymentEvent = null!;

    [SetUp]
    public async Task Setup()
    {
        // Arrange
        _finalisedOnProgammeLearningPaymentEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        var command = new CalculateRequiredLevyAmountCommand(_finalisedOnProgammeLearningPaymentEvent);

        _messageSession = new Mock<IMessageSession>();
        _sut = new CalculateRequiredLevyAmountCommandHandler(_messageSession.Object,
            Mock.Of<ILogger<CalculateRequiredLevyAmountCommandHandler>>());

        // Act
        await _sut.Process(command);
    }

    [Test] // Assert
    public void Process_Publishes_CalculatedRequiredLevyAmountEvent()
    {
        // AccountId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AccountId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.EmployingAccountId),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ActualEndDate
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ActualEndDate == _finalisedOnProgammeLearningPaymentEvent.ActualEndDate),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // AgreedOnDate
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AgreedOnDate == null),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // AgreementId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AgreementId == null),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // AmountDue
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AmountDue == _finalisedOnProgammeLearningPaymentEvent.Amount),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ApprenticeshipEmployerType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipEmployerType == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ApprenticeshipId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.FundingCommitmentId),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ApprenticeshipPriceEpisodeId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipPriceEpisodeId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.FundingPeriodId),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ClawbackSourcePaymentEventId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ClawbackSourcePaymentEventId == null),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // CollectionPeriod.AcademicYear
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.CollectionPeriod.AcademicYear == _finalisedOnProgammeLearningPaymentEvent.CollectionYear),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // CompletionStatus
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.CompletionStatus == 1),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ContractType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ContractType == ContractType.Act1),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // DeliveryPeriod
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.DeliveryPeriod == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.DeliveryPeriod),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // EarningEventId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EarningEventId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.ApprenticeshipEarningsId),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // EventId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EventId != Guid.Empty),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // EventTime
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EventTime < DateTime.UtcNow && e.EventTime > DateTime.UtcNow.AddSeconds(-5)),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // IlrFileName
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.IlrFileName == null),
            It.IsAny<PublishOptions>()),
            Times.Once());

        //// IlrSubmissionDateTime
        //_messageSession.Verify(ms => ms.Publish(
        //    It.Is<CalculatedRequiredLevyAmount>(e => e.IlrSubmissionDateTime == null),
        //    It.IsAny<PublishOptions>()),
        //    Times.Once());

        // InstalmentAmount
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.InstalmentAmount == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.DeliveryPeriodAmount),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // JobId
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.JobId == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // Learner.ReferenceNumber
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.ReferenceNumber == null),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // Learner.Uln
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.Uln == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.Uln),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.FrameworkCode
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FrameworkCode == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.FundingLineType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FundingLineType == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.FundingLineType),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.PathwayCode
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.PathwayCode == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.ProgrammeType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.ProgrammeType == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.Reference
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.Reference == "ZPROG001"),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.SequenceNumber
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.SequenceNumber == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.StandardCode
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StandardCode == _finalisedOnProgammeLearningPaymentEvent.CourseCode),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAim.StartDate
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningAimSequenceNumber
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAimSequenceNumber == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // LearningStartDate
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningStartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // NumberOfInstalments
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.NumberOfInstalments == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.NumberOfInstalments),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // OnProgrammeEarningType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.OnProgrammeEarningType == OnProgrammeEarningType.Learning),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // PlannedEndDate
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.PlannedEndDate == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.PlannedEndDate),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // PriceEpisodeIdentifier
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.PriceEpisodeIdentifier == ""),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // Priority
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Priority == 0),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // ReportingAimFundingLineType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ReportingAimFundingLineType == ""),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // SfaContributionPercentage
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.SfaContributionPercentage == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.GovernmentContributionPercentage),
            It.IsAny<PublishOptions>()),
            Times.Once());

        // TransactionType
        _messageSession.Verify(ms => ms.Publish(
            It.Is<CalculatedRequiredLevyAmount>(e => e.TransactionType == TransactionType.Learning),
            It.IsAny<PublishOptions>()),
            Times.Once());
    }
}