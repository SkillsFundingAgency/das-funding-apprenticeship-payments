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
    private FinalisedOnProgammeLearningPaymentEventBuilder _sut;
    private FinalisedOnProgammeLearningPaymentEvent _result;
    private Fixture _fixture;
    private PaymentEntityModel _paymentEntityModel;
    private Guid _apprenticeshipKey;
    private short _totalNumberOfPayments;

    [SetUp]
    public void SetUp()
    {
        _sut = new FinalisedOnProgammeLearningPaymentEventBuilder();
        _fixture = new Fixture();

        _paymentEntityModel = _fixture.Create<PaymentEntityModel>();
        _apprenticeshipKey = Guid.NewGuid();
        _totalNumberOfPayments = _fixture.Create<short>();

        _result = _sut.Build(_paymentEntityModel, _apprenticeshipKey, _totalNumberOfPayments);
    }

    [Test]
    public void ShouldPopulate_TheApprenticeshipKey_Correctly()
    {
        _result.ApprenticeshipKey.Should().Be(_apprenticeshipKey);
    }

    [Test]
    public void ShouldPopulate_TheCollectionYear_Correctly()
    {
        _result.CollectionYear.Should().Be(_paymentEntityModel.CollectionYear);
    }

    [Test]
    public void ShouldPopulate_TheCollectionMonth_Correctly()
    {
        _result.CollectionMonth.Should().Be(_paymentEntityModel.CollectionPeriod);
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
        _result.ApprenticeshipEarnings.NumberOfInstalments.Should().Be(_totalNumberOfPayments);
    }

    [Test, Ignore("TODO")]
    public void ShouldPopulate_AccountId_Correctly()
    {
        _result.AccountId.Should().Be(123456);
    }

    [Test, Ignore("TODO")]
    public void ShouldPopulate_TransferSenderAccountId_Correctly()
    {
        _result.EmployerDetails.FundingAccountId.Should().Be(123456);
    }

    [Test]
    public void ShouldPopulate_DeliveryPeriodAmount_Correctly()
    {
        _result.ApprenticeshipEarnings.DeliveryPeriodAmount.Should().Be(_paymentEntityModel.Amount);
    }


    [Test, Ignore("TODO")]
    public void ShouldPopulate_FundingCommitmentId_Correctly()
    {
        _result.EmployerDetails.FundingCommitmentId.Should().Be(1234);
    }

    [Test, Ignore("TODO")]
    public void ShouldPopulate_ApprenticeshipEmployerType_Correctly()
    {
        // ??????????????? non levy const?
    }

    [Test, Ignore("TODO")]
    public void ShouldPopulate_CourseCode_Correctly()
    {
        _result.CourseCode.Should().Be(545454);
    }
}