using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests;

public class FinalisedOnProgammeLearningPaymentEventBuilder_BuildTests
{
    private FinalisedOnProgammeLearningPaymentEventBuilder _sut = null!;
    private FinalisedOnProgammeLearningPaymentEvent _result = null!;
    private Fixture _fixture = null!;
    private PaymentEntityModel _paymentEntityModel = null!;
    private ApprenticeshipEntityModel _apprenticeship = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new FinalisedOnProgammeLearningPaymentEventBuilder();
        _fixture = new Fixture();

        _paymentEntityModel = _fixture.Create<PaymentEntityModel>();
        _apprenticeship = _fixture.Create<ApprenticeshipEntityModel>();

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
        _result.ApprenticeshipEarnings.GovernmentContributionPercentage.Should().Be(0.95m);
    }

    [Test]
    public void ShouldPopulate_EarningEventId_Correctly()
    {
        _result.ApprenticeshipEarnings.ApprenticeshipEarningsId.Should().NotBe(Guid.Empty);
    }

    [Test]
    public void ShouldPopulate_NumberOfInstalments_Correctly()
    {
        _result.ApprenticeshipEarnings.NumberOfInstalments.Should().Be((short)_apprenticeship.Earnings.Count);
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
        _result.ApprenticeshipEarnings.DeliveryPeriodAmount.Should().Be(_paymentEntityModel.Amount);
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
}