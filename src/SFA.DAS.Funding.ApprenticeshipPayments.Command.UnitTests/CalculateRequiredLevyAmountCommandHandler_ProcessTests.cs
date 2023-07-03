using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class CalculateRequiredLevyAmountCommandHandler_ProcessTests
{
    private readonly Fixture _fixture = new();
    private Mock<IPaymentsV2ServiceBusEndpoint> _busEndpoint = null!;
    private CalculateRequiredLevyAmountCommandHandler _sut = null!;
    private FinalisedOnProgammeLearningPaymentEvent _finalisedOnProgammeLearningPaymentEvent = null!;

    [SetUp]
    public async Task Setup()
    {
        // Arrange
        _finalisedOnProgammeLearningPaymentEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        var command = new CalculateRequiredLevyAmountCommand(_finalisedOnProgammeLearningPaymentEvent);

        _busEndpoint = new Mock<IPaymentsV2ServiceBusEndpoint>();
        _sut = new CalculateRequiredLevyAmountCommandHandler(_busEndpoint.Object,
            Mock.Of<ILogger<CalculateRequiredLevyAmountCommandHandler>>());

        // Act
        await _sut.Process(command);
    }

    [Test] // Assert
    public void Process_Sends_CalculatedRequiredLevyAmount()
    {
        // AccountId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AccountId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.EmployingAccountId)),
            Times.Once());

        // ActualEndDate
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ActualEndDate == null)),
            Times.Once());

        // AgreedOnDate
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AgreedOnDate == null)),
            Times.Once());

        // AgreementId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AgreementId == null)),
            Times.Once());

        // AmountDue
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AmountDue == _finalisedOnProgammeLearningPaymentEvent.Amount)),
            Times.Once());

        // ApprenticeshipEmployerType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipEmployerType == 0)),
            Times.Once());

        // ApprenticeshipId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.FundingCommitmentId)),
            Times.Once());

        // ApprenticeshipPriceEpisodeId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipPriceEpisodeId == null)),
            Times.Once());

        // ClawbackSourcePaymentEventId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ClawbackSourcePaymentEventId == null)),
            Times.Once());

        // CollectionPeriod.AcademicYear
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.CollectionPeriod.AcademicYear == _finalisedOnProgammeLearningPaymentEvent.CollectionYear)),
            Times.Once());

        // CompletionStatus
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.CompletionStatus == 1)),
            Times.Once());

        // ContractType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ContractType == ContractType.Act1)),
            Times.Once());

        // DeliveryPeriod
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.DeliveryPeriod == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.DeliveryPeriod)),
            Times.Once());

        // EarningEventId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EarningEventId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.ApprenticeshipEarningsId)),
            Times.Once());

        // EventId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EventId != Guid.Empty)),
            Times.Once());

        // EventTime
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EventTime < DateTime.UtcNow && e.EventTime > DateTime.UtcNow.AddSeconds(-5))),
            Times.Once());

        // IlrFileName
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.IlrFileName == null)),
            Times.Once());

        //// IlrSubmissionDateTime
        //_busEndpoint.Verify(ms => ms.Send(
        //    It.Is<CalculatedRequiredLevyAmount>(e => e.IlrSubmissionDateTime == null)),
        //    It.IsAny<SendOptions>()),
        //    Times.Once());

        // InstalmentAmount
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.InstalmentAmount == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.DeliveryPeriodAmount)),
            Times.Once());

        // JobId
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.JobId == 0)),
            Times.Once());

        // Learner.ReferenceNumber
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.ReferenceNumber == null)),
            Times.Once());

        // Learner.Uln
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.Uln == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.Uln)),
            Times.Once());

        // LearningAim.FrameworkCode
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FrameworkCode == 0)),
            Times.Once());

        // LearningAim.FundingLineType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FundingLineType == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.FundingLineType)),
            Times.Once());

        // LearningAim.PathwayCode
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.PathwayCode == 0)),
            Times.Once());

        // LearningAim.ProgrammeType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.ProgrammeType == 0)),
            Times.Once());

        // LearningAim.Reference
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.Reference == "ZPROG001")),
            Times.Once());

        // LearningAim.SequenceNumber
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.SequenceNumber == 0)),
            Times.Once());

        // LearningAim.StandardCode
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StandardCode == _finalisedOnProgammeLearningPaymentEvent.CourseCode)),
            Times.Once());

        // LearningAim.StartDate
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());

        // LearningAimSequenceNumber
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAimSequenceNumber == 0)),
            Times.Once());

        // LearningStartDate
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningStartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());

        // NumberOfInstalments
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.NumberOfInstalments == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.NumberOfInstalments)),
            Times.Once());

        // OnProgrammeEarningType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.OnProgrammeEarningType == OnProgrammeEarningType.Learning)),
            Times.Once());

        // PlannedEndDate
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.PlannedEndDate == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.PlannedEndDate)),
            Times.Once());

        // PriceEpisodeIdentifier
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.PriceEpisodeIdentifier == "")),
            Times.Once());

        // Priority
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Priority == 0)),
            Times.Once());

        // ReportingAimFundingLineType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ReportingAimFundingLineType == "")),
            Times.Once());

        // SfaContributionPercentage
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.SfaContributionPercentage == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.GovernmentContributionPercentage)),
            Times.Once());

        // TransactionType
        _busEndpoint.Verify(ms => ms.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.TransactionType == TransactionType.Learning)),
            Times.Once());
    }
}