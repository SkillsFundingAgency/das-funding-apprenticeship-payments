using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

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

    [Test]
    public void Process_Publishes_CalculatedRequiredLevyAmountEvent()
    {
        // Assert
        _messageSession.Verify(ms => ms.Publish(It.Is<CalculatedRequiredLevyAmountEvent>(e =>
                e.AccountId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.EmployingAccountId
               && e.ActualEndDate == _finalisedOnProgammeLearningPaymentEvent.ActualEndDate
              &&  e.AgreedOnDate == null
           && e.AgreementId == null
           && e.AmountDue == _finalisedOnProgammeLearningPaymentEvent.Amount
           && e.ApprenticeshipEmployerType == 0
           && e.ApprenticeshipId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.FundingCommitmentId
           && e.ApprenticeshipPriceEpisodeId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.FundingPeriodId
           && e.ClawbackSourcePaymentEventId == null
           && e.CollectionPeriod.AcademicYear == _finalisedOnProgammeLearningPaymentEvent.CollectionYear
           // && e.CollectionPeriod.Period == _finalisedOnProgammeLearningPaymentEvent.Period // TODO:
           && e.CompletionStatus == 1
           && e.ContractType == "ACT1"
           && e.DeliveryPeriod == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.DeliveryPeriod
           && e.EarningEventId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.ApprenticeshipEarningsId
           && e.EventId != Guid.Empty
           && e.EventTime < DateTime.UtcNow
           && e.EventTime > DateTime.UtcNow.AddSeconds(-5)
           && e.IlrFileName == null
           && e.IlrSubmissionDateTime == null
           && e.InstalmentAmount == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.DeliveryPeriodAmount
           && e.JobId == "TBC"
           && e.Learner.ReferenceNumber == null
           && e.Learner.Uln == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.Uln
           && e.LearningAim.FrameworkCode == 0
           && e.LearningAim.FundingLineType == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.FundingLineType
           && e.LearningAim.PathwayCode == 0
           && e.LearningAim.ProgrammeType == 0
           && e.LearningAim.Reference == "ZPROG001"
           && e.LearningAim.SequenceNumber == 0
           && e.LearningAim.StandardCode == _finalisedOnProgammeLearningPaymentEvent.CourseCode
           && e.LearningAim.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate
           && e.LearningAimSequenceNumber == 0
           && e.LearningStartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate
           && e.NumberOfInstalments == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.NumberOfInstalments
           && e.OnProgrammeEarningType == OnProgrammeEarningType.Learning
           && e.PlannedEndDate == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.PlannedEndDate
           && e.PriceEpisodeIdentifier == ""
           && e.Priority == 0
           && e.ReportingAimFundingLineType == ""
           && e.SfaContributionPercentage == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.GovernmentContributionPercentage
           && e.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate
           && e.TransactionType == TransactionType.Learning
           && e.TransferSenderAccountId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.FundingAccountId
           && e.Ukprn == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarnings.ProviderIdentifier
           && e.EarningSource == "SubmitLearnerDataFundingPlatform"
            ), It.IsAny<PublishOptions>())

        , Times.Once());
    }
}