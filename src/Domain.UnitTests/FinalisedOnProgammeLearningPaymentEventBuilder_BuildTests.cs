using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests;

public class FinalisedOnProgammeLearningPaymentEventBuilder_BuildTests
{
    private FinalisedOnProgammeLearningPaymentEventBuilder _sut = null!;
    private FinalisedOnProgammeLearningPaymentEvent _result = null!;
    private Fixture _fixture = null!;
    private Domain.Apprenticeship.Payment _paymentEntityModel = null!;
    private Domain.Apprenticeship.Apprenticeship _apprenticeship = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new FinalisedOnProgammeLearningPaymentEventBuilder();
        _fixture = new Fixture();
        _fixture.Customize(new EarningsGeneratedEventCustomization());

        _paymentEntityModel = _fixture.Create<Domain.Apprenticeship.Payment>();
        _apprenticeship = new Domain.Apprenticeship.Apprenticeship(_fixture.Create<EarningsGeneratedEvent>());
        _apprenticeship.CalculatePayments(DateTime.Now);

        _result = _sut.Build(_paymentEntityModel, _apprenticeship);
    }

    [Test]
    public void ShouldPopulate_TheApprenticeshipKey_Correctly()
    {
        _result.ApprenticeshipKey.Should().Be(_apprenticeship.ApprenticeshipKey);
    }

    [Test]
    public void ShouldPopulate_TheCollectionYear_Correctly()
    {
        _result.CollectionYear.Should().Be(_paymentEntityModel.CollectionYear);
    }

    [Test]
    public void ShouldPopulate_TheCollectionMonth_Correctly()
    {
        _result.CollectionPeriod.Should().Be(_paymentEntityModel.CollectionPeriod);
    }

    [Test]
    public void ShouldPopulate_TheAmount_Correctly()
    {
        _result.Amount.Should().Be(_paymentEntityModel.Amount);
    }

    [Test]
    public void ShouldPopulate_GovernmentContributionPercentage_Correctly()
    {
        _result.ApprenticeshipEarning.GovernmentContributionPercentage.Should().Be(0.95m);
    }

    [Test]
    public void WhenUnder22AndNoneLevy_ShouldPopulate_GovernmentContributionPercentage_Correctly()
    {
        var earningsGeneratedEvent = _fixture.Create<EarningsGeneratedEvent>();
        earningsGeneratedEvent.EmployerType = EmployerType.NonLevy;
        earningsGeneratedEvent.AgeAtStartOfApprenticeship = 21;

        var apprenticeship = new Domain.Apprenticeship.Apprenticeship(earningsGeneratedEvent);

        var result = _sut.Build(_paymentEntityModel, apprenticeship);
        result.ApprenticeshipEarning.GovernmentContributionPercentage.Should().Be(1);
    }

    [Test]
    public void ShouldPopulate_EarningEventId_Correctly()
    {
        _result.ApprenticeshipEarning.ApprenticeshipEarningsId.Should().NotBe(Guid.Empty);
    }

    [Test]
    public void ShouldPopulate_NumberOfInstalments_Correctly()
    {
        _result.ApprenticeshipEarning.NumberOfInstalments.Should().Be((short)_apprenticeship.Earnings.Count);
    }

    [Test]
    public void ShouldPopulate_EmployingAccountId_Correctly()
    {
        _result.EmployerDetails.EmployingAccountId.Should().Be(_apprenticeship.FundingEmployerAccountId);
    }

    [Test]
    public void ShouldPopulate_TransferSenderAccountId_Correctly()
    {
        _result.EmployerDetails.FundingAccountId.Should().Be(_apprenticeship.TransferSenderAccountId);
    }

    [Test]
    public void ShouldPopulate_DeliveryPeriodAmount_Correctly()
    {
        _result.ApprenticeshipEarning.DeliveryPeriodAmount.Should().Be(_paymentEntityModel.Amount);
    }

    [Test]
    public void ShouldPopulate_FundingCommitmentId_Correctly()
    {
        _result.EmployerDetails.FundingCommitmentId.Should().Be(_apprenticeship.FundingCommitmentId);
    }

    [Test]
    public void ShouldPopulate_ApprenticeshipEmployerType_Correctly()
    {
        _result.ApprenticeshipEmployerType.Should().Be(_apprenticeship.EmployerType);
    }

    [Test]
    public void ShouldPopulate_CourseCode_Correctly()
    {
        _result.CourseCode.Should().Be(_apprenticeship.CourseCode);
    }

    [Test]
    public void ShouldPopulate_StartDate_Correctly()
    {
        _result.Apprenticeship.StartDate.Should().Be(_apprenticeship.StartDate);
    }

    [Test]
    public void ShouldPopulate_Uln_Correctly()
    {
        _result.ApprenticeshipEarning.Uln.Should().Be(_apprenticeship.Uln);
    }

    [Test]
    public void ShouldPopulate_LearnerReference_Correctly()
    {
        _result.ApprenticeshipEarning.LearnerReference.Should().Be(_apprenticeship.LearnerReference);
    }

    [Test]
    public void ShouldPopulate_PlannedEndDate_Correctly()
    {
        _result.ApprenticeshipEarning.PlannedEndDate.Should().Be(_apprenticeship.PlannedEndDate);
    }

    [Test]
    public void ShouldPopulate_ProviderIdentifier_Correctly()
    {
        _result.ApprenticeshipEarning.ProviderIdentifier.Should().Be(_apprenticeship.Ukprn);
    }

    [Test]
    public void ShouldPopulate_ApprovalsApprenticeshipId_Correctly()
    {
        _result.Apprenticeship.ApprovalsApprenticeshipId.Should().Be(_apprenticeship.ApprovalsApprenticeshipId);
    }
    
    [Test]
    public void ShouldPopulate_TheEarningsProfileId_Correctly()
    {
        _result.EarningsProfileId.Should().Be(_paymentEntityModel.EarningsProfileId);
    }
}