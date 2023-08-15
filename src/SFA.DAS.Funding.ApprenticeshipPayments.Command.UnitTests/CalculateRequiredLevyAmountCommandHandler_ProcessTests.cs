using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
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
    private short _academicYear;
    private short _academicYearStartingYear;

    [SetUp]
    public async Task Setup()
    {
        // Arrange
        _academicYear = 1718;
        _academicYearStartingYear = 2017;

        _finalisedOnProgammeLearningPaymentEvent = _fixture.Build<FinalisedOnProgammeLearningPaymentEvent>()
            .With(_ => _.CourseCode, _fixture.Create<short>().ToString)
            .With(_ => _.CollectionYear, _academicYear)
            .Create();

        var command = new CalculateRequiredLevyAmountCommand(_finalisedOnProgammeLearningPaymentEvent);

        _busEndpoint = new Mock<IPaymentsV2ServiceBusEndpoint>();
        _sut = new CalculateRequiredLevyAmountCommandHandler(_busEndpoint.Object,
            Mock.Of<ILogger<CalculateRequiredLevyAmountCommandHandler>>());

        // Act
        await _sut.Send(command);
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_AccountId()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.AccountId == _finalisedOnProgammeLearningPaymentEvent.EmployerDetails.EmployingAccountId)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ActualEndDate()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ActualEndDate == null)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_AgreedOnDate()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.AgreedOnDate == null)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_AgreementId()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.AgreementId == null)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_AmountDue()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.AmountDue == _finalisedOnProgammeLearningPaymentEvent.Amount)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ApprenticeshipEmployerType()
    {
        var expected = _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEmployerType == 
                       EmployerType.Levy ? ApprenticeshipEmployerType.Levy : ApprenticeshipEmployerType.NonLevy;
        _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEmployerType = EmployerType.NonLevy;

        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipEmployerType == expected)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ApprenticeshipId()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipId == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.ApprovalsApprenticeshipId)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ApprenticeshipPriceEpisodeId_Null()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ApprenticeshipPriceEpisodeId == null)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ClawbackSourcePaymentEventId_Null()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ClawbackSourcePaymentEventId == null)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_CollectionPeriodAcademicYear_MatchesFinalisedOnProgammeLearningPaymentEventCollectionYear()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.CollectionPeriod.AcademicYear == _finalisedOnProgammeLearningPaymentEvent.CollectionYear)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_CompletionStatus_1()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.CompletionStatus == 1)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ContractType_Act1()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ContractType == ContractType.Act1)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_DeliveryPeriod_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsDeliveryPeriod()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.DeliveryPeriod == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.DeliveryPeriod)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_EarningEventId_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsId()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.EarningEventId == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.ApprenticeshipEarningsId)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_EventId_NotEmpty()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.EventId != Guid.Empty)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_EventTime_InCorrectRange()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.EventTime < DateTime.UtcNow && e.EventTime > DateTime.UtcNow.AddSeconds(-5))),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_IlrFileName_AsEmptyString()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.IlrFileName == "")),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_IlrSubmissionDateTime_Null()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e =>
                    e.IlrSubmissionDateTime.Year == _academicYearStartingYear
                    && e.IlrSubmissionDateTime.Month == 8
                    && e.IlrSubmissionDateTime.Day == 1)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_InstalmentAmount_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsDeliveryPeriodAmount()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.InstalmentAmount == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.DeliveryPeriodAmount)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_JobId_NegativeOne()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.JobId == -1)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearnerReferenceNumber_NonEmpty12DigitNumber()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.ReferenceNumber.Length == 12 && !e.Learner.ReferenceNumber.IsNullOrEmpty())),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearnerUln_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsUln()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.Learner.Uln == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.Uln)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimFrameworkCode_0()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FrameworkCode == 0)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimFundingLineType_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsFundingLineType()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.FundingLineType == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.FundingLineType)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimPathwayCode_0()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.PathwayCode == 0)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimProgrammeType_0()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.ProgrammeType == 0)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimReference_MatchesFinalisedOnProgammeLearningPaymentEventCourseCode()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.Reference == "ZPROG001")),
            Times.Once());
    }


    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimStandardCode_MatchesFinalisedOnProgammeLearningPaymentEventCourseCode()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StandardCode.ToString() == _finalisedOnProgammeLearningPaymentEvent.CourseCode)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimStartDate_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipStartDate()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningAimSequenceNumber_0()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningAim.SequenceNumber == 0)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_LearningStartDate_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipStartDate()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.LearningStartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_NumberOfInstalments_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsNumberOfInstalments()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.NumberOfInstalments == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.NumberOfInstalments)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_OnProgrammeEarningType_Learning()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.OnProgrammeEarningType == OnProgrammeEarningType.Learning)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_PlannedEndDate_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipEarningsPlannedEndDate()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.PlannedEndDate == _finalisedOnProgammeLearningPaymentEvent.ApprenticeshipEarning.PlannedEndDate)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_PriceEpisodeIdentifier_EmptyString()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.PriceEpisodeIdentifier == "")),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_Priority_0()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.Priority == 0)),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_ReportingAimFundingLineType_EmptyString()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.ReportingAimFundingLineType == "")),
            Times.Once());
    }

    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_StartDate_MatchesFinalisedOnProgammeLearningPaymentEventApprenticeshipStartDate()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.StartDate == _finalisedOnProgammeLearningPaymentEvent.Apprenticeship.StartDate)),
            Times.Once());
    }


    [Test]
    public void Process_Sends_CalculatedRequiredLevyAmount_TransactionType()
    {
        _busEndpoint.Verify(ms => ms.Send(
                It.Is<CalculatedRequiredLevyAmount>(e => e.TransactionType == TransactionType.Learning)),
            Times.Once());
    }

}