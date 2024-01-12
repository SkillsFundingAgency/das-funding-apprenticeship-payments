using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests;

public class ApprenticeshipEntity_HandleEarningsRecalculatedEventTests
{
    private ApprenticeshipEntity _sut = null!;
    private ApprenticeshipEarningsRecalculatedEvent _earningsRecalculatedEvent = null!;
    private Mock<IRecalculateApprenticeshipPaymentsCommandHandler> _recalculateApprenticeshipPaymentsCommandHandler = null!;
    private Fixture _fixture = null!;
    private Apprenticeship _apprenticeship = null!;
    private IEnumerable<EarningEntityModel> _expectedEarnings = null!;
    private IEnumerable<PaymentEntityModel> _expectedPayments = null!;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();

        _earningsRecalculatedEvent = _fixture.Create<ApprenticeshipEarningsRecalculatedEvent>();

        _apprenticeship = new Apprenticeship(Guid.NewGuid(), _fixture.CreateMany<Earning>().ToList(), _fixture.CreateMany<Payment>().ToList());

        _expectedEarnings = _apprenticeship.Earnings.Select(x => new EarningEntityModel
        {
            AcademicYear = x.AcademicYear,
            Amount = x.Amount,
            CollectionMonth = x.CollectionMonth,
            CollectionYear = x.CollectionYear,
            DeliveryPeriod = x.DeliveryPeriod,
            FundingLineType = x.FundingLineType,
            EarningsProfileId = x.EarningsProfileId
        });

        _expectedPayments = _apprenticeship.Payments.Select(x => new PaymentEntityModel
        {
            CollectionYear = x.CollectionYear,
            DeliveryPeriod = x.DeliveryPeriod,
            FundingLineType = x.FundingLineType,
            AcademicYear = x.AcademicYear,
            Amount = x.Amount,
            SentForPayment = x.SentForPayment,
            CollectionPeriod = x.CollectionPeriod,
            EarningsProfileId = x.EarningsProfileId
        });

        _recalculateApprenticeshipPaymentsCommandHandler = new Mock<IRecalculateApprenticeshipPaymentsCommandHandler>();
        _sut = new ApprenticeshipEntity(Mock.Of<ICalculateApprenticeshipPaymentsCommandHandler>(), Mock.Of<IProcessUnfundedPaymentsCommandHandler>(), _recalculateApprenticeshipPaymentsCommandHandler.Object, Mock.Of<ILogger<ApprenticeshipEntity>>());
        _sut.Model = new ApprenticeshipEntityModel();

        _recalculateApprenticeshipPaymentsCommandHandler.Setup(x => x.Recalculate(It.IsAny<RecalculateApprenticeshipPaymentsCommand>())).ReturnsAsync(_apprenticeship);

        await _sut.HandleEarningsRecalculatedEvent(_earningsRecalculatedEvent);
    }

    [Test]
    public void ShouldCallGenerateEarnings()
    {
        _recalculateApprenticeshipPaymentsCommandHandler.Verify(x => x.Recalculate(It.Is<RecalculateApprenticeshipPaymentsCommand>(y => y.ApprenticeshipEntity == _sut.Model)));
    }

    [Test]
    public void ShouldMapEarnings()
    {
        _sut.Model.Earnings.Should().BeEquivalentTo(_expectedEarnings);
    }

    [Test]
    public void ShouldMapPayments()
    {
        _sut.Model.Payments.Should().BeEquivalentTo(_expectedPayments);
    }
}