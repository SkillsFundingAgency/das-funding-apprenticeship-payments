using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

[TestFixture]
public class CalculateRequiredLevyAmountCommandHandler_ProcessTests
{
    private readonly Fixture _fixture = new();
    private Mock<IPaymentsV2ServiceBusEndpoint> _endpoint = null!;
    private CalculateRequiredLevyAmountCommandHandler _sut = null!;
    private FinalisedOnProgammeLearningPaymentEvent _finalisedOnProgammeLearningPaymentEvent = null!;

    [SetUp]
    public async Task Setup()
    {
        // Arrange
        _finalisedOnProgammeLearningPaymentEvent = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();
        var command = new CalculateRequiredLevyAmountCommand(_finalisedOnProgammeLearningPaymentEvent);

        _endpoint = new Mock<IPaymentsV2ServiceBusEndpoint>();
        _sut = new CalculateRequiredLevyAmountCommandHandler(_endpoint.Object,
            Mock.Of<ILogger<CalculateRequiredLevyAmountCommandHandler>>());

        // Act
        await _sut.Process(command);
    }

    [Test] // Assert
    public void Process_Publishes_CalculatedRequiredLevyAmount()
    {
        // AccountId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AccountId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.EmployingAccountId)),
            Times.Once());

        // ActualEndDate
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ActualEndDate == null)),
            Times.Once());

        // AgreedOnDate
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AgreedOnDate == null)),
            Times.Once());

        // AgreementId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AgreementId == null)),
            Times.Once());

        // AmountDue
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.AmountDue == _finalisedOnProgammeLearningPaymentEvent.Amount)),
            Times.Once());

        // ApprenticeshipEmployerType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipEmployerType == 0)),
            Times.Once());

        // ApprenticeshipId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.FundingCommitmentId)),
            Times.Once());

        // ApprenticeshipPriceEpisodeId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipPriceEpisodeId == null)),
            Times.Once());

        // ClawbackSourcePaymentEventId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ClawbackSourcePaymentEventId == null)),
            Times.Once());

        // CollectionPeriod.AcademicYear
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.CollectionPeriod.AcademicYear == _finalisedOnProgammeLearningPaymentEvent.CollectionYear)),
            Times.Once());

        // CompletionStatus
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.CompletionStatus == 1)),
            Times.Once());

        // ContractType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ContractType == ContractType.Act1)),
            Times.Once());

        // DeliveryPeriod
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.DeliveryPeriod == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.DeliveryPeriod)),
            Times.Once());

        // EarningEventId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EarningEventId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.ApprenticeshipEarningsId)),
            Times.Once());

        // EventId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EventId != Guid.Empty)),
            Times.Once());

        // EventTime
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.EventTime < DateTime.UtcNow && e.EventTime > DateTime.UtcNow.AddSeconds(-5))),
            Times.Once());

        // InstalmentAmount
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.InstalmentAmount == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.DeliveryPeriodAmount)),
            Times.Once());

        // JobId
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.JobId == 0)),
            Times.Once());

        // Learner.ReferenceNumber
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.ReferenceNumber == null)),
            Times.Once());

        // Learner.Uln
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.Uln == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.Uln)),
            Times.Once());

        // LearningAim.FrameworkCode
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FrameworkCode == 0)),
            Times.Once());

        // LearningAim.FundingLineType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FundingLineType == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.FundingLineType)),
            Times.Once());

        // LearningAim.PathwayCode
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.PathwayCode == 0)),
            Times.Once());

        // LearningAim.ProgrammeType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.ProgrammeType == 0)),
            Times.Once());

        // LearningAim.Reference
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.Reference == "ZPROG001")),
            Times.Once());

        // LearningAim.SequenceNumber
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.SequenceNumber == 0)),
            Times.Once());

        // LearningAim.StandardCode
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StandardCode == _finalisedOnProgammeLearningPaymentEvent.CourseCode)),
            Times.Once());

        // LearningAim.StartDate
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());

        // LearningAimSequenceNumber
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAimSequenceNumber == 0)),
            Times.Once());

        // LearningStartDate
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.LearningStartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());

        // NumberOfInstalments
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.NumberOfInstalments == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.NumberOfInstalments)),
            Times.Once());

        // OnProgrammeEarningType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.OnProgrammeEarningType == OnProgrammeEarningType.Learning)),
            Times.Once());

        // PlannedEndDate
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.PlannedEndDate == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.PlannedEndDate)),
            Times.Once());

        // PriceEpisodeIdentifier
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.PriceEpisodeIdentifier == "")),
            Times.Once());

        // Priority
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.Priority == 0)),
            Times.Once());

        // ReportingAimFundingLineType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.ReportingAimFundingLineType == "")),
            Times.Once());

        // SfaContributionPercentage
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.SfaContributionPercentage == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.GovernmentContributionPercentage)),
            Times.Once());

        // TransactionType
        _endpoint.Verify(ep => ep.Send(
            It.Is<CalculatedRequiredLevyAmount>(e => e.TransactionType == TransactionType.Learning)),
            Times.Once());
    }
}