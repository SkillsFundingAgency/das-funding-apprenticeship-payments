using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Factories;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using Apprenticeship = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Apprenticeship;
using Payment = SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship.Payment;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class RecalculateApprenticeshipPaymentsCommandHandler_CalculateTests
{
    private RecalculateApprenticeshipPaymentsCommand _command = null!;
    private Fixture _fixture = null!;
    private RecalculateApprenticeshipPaymentsCommandHandler _sut = null!;
    private ApprenticeshipEntityModel _apprenticeshipEntityModel = null!;
    private List<Earning> _newEarnings = null!;
    private List<Earning> _existingEarnings = null!;
    private List<Payment> _existingPayments = null!;
    private Guid _previousEarningsProfileId = Guid.NewGuid();
    private Guid _newEarningsProfileId = Guid.NewGuid();
    private Mock<IApprenticeshipFactory> _apprenticeshipFactory = null!;
    private Mock<IDasServiceBusEndpoint> _busEndpoint = null!;
    private Mock<IPaymentsGeneratedEventBuilder> _paymentsGeneratedEventBuilder = null!;
    private Apprenticeship _result;
    private decimal _currentMonthlyLearningAmount;
    private decimal _newMonthlyLearningAmount;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();
        _apprenticeshipEntityModel = new ApprenticeshipEntityModel{ ApprenticeshipKey = Guid.NewGuid() };
        _newEarnings = _fixture.CreateMany<Earning>().ToList();
        _existingEarnings = _fixture.CreateMany<Earning>().ToList();
        _existingPayments = _fixture.CreateMany<Payment>().ToList();
        _currentMonthlyLearningAmount = _fixture.Create<decimal>();
        _newMonthlyLearningAmount = _fixture.Create<decimal>();

        //Expectation:
        //Delivery Period 1 - diff payment calculated
        //Delivery Period 2 - unpaid payment removed and new one calculated for new learning amount
        //Delivery Period 3 - no existing payments, new one calculated for new learning amount
        _newEarnings = new List<Earning>
        {
            new Earning(2223, 1, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId),
            new Earning(2223, 2, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId),
            new Earning(2223, 3, _newMonthlyLearningAmount, 2022, 10, _fixture.Create<string>(), _newEarningsProfileId)
        };

        _existingPayments = new List<Payment>()
        {
            new Payment(2223, 1, _currentMonthlyLearningAmount, 2022, 8, "payment 1 paid", _previousEarningsProfileId){ SentForPayment = true },
            new Payment(2223, 2, _currentMonthlyLearningAmount, 2022, 8, "payment 2 unpaid", _previousEarningsProfileId)
        };

        _command = new RecalculateApprenticeshipPaymentsCommand(_apprenticeshipEntityModel, _newEarnings);

        _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
        _busEndpoint = new Mock<IDasServiceBusEndpoint>();
        _paymentsGeneratedEventBuilder = new Mock<IPaymentsGeneratedEventBuilder>();

        _apprenticeshipFactory.Setup(x => x.LoadExisting(It.IsAny<ApprenticeshipEntityModel>()))
            .Returns(new Apprenticeship(_apprenticeshipEntityModel.ApprenticeshipKey, _existingEarnings, _existingPayments));

        _paymentsGeneratedEventBuilder.Setup(x => x.Build(It.IsAny<Apprenticeship>()))
            .Returns(new PaymentsGeneratedEvent());

        _sut = new RecalculateApprenticeshipPaymentsCommandHandler(
            _apprenticeshipFactory.Object,
            _busEndpoint.Object,
            _paymentsGeneratedEventBuilder.Object,
            Mock.Of<ILogger<CalculateApprenticeshipPaymentsCommandHandler>>());
        _result = await _sut.Recalculate(_command);
    }

    [Test]
    public void ThenTheEarningsAreReplaced()
    {
        _result.Earnings.Count.Should().Be(_newEarnings.Count);

        foreach (var expectedEarning in _newEarnings)
        {
            _result.Earnings.Should().Contain(x =>
                x.AcademicYear == expectedEarning.AcademicYear
                && x.FundingLineType == expectedEarning.FundingLineType
                && x.Amount == expectedEarning.Amount
                && x.CollectionMonth == expectedEarning.CollectionMonth
                && x.CollectionYear == expectedEarning.CollectionYear
                && x.DeliveryPeriod == expectedEarning.DeliveryPeriod
                && x.EarningsProfileId == _newEarningsProfileId);
        }
    }

    [Test]
    public void ExistingUnpaidPaymentsShouldBeRemoved()
    {
        _result.Payments.Should().NotContain(p => p.FundingLineType == "payment 2 unpaid");
    }

    [Test]
    public void ExistingPaidPaymentsShouldBeNotBeRemoved()
    {
        _result.Payments.Should().Contain(p => p.FundingLineType == "payment 1 paid");
    }

    [Test]
    public void NewPaymentsToMakeUpExistingPaidPaymentsAmountsShouldBeAdded()
    {
        _result.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 1 && p.Amount == _newMonthlyLearningAmount - _currentMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void NewPaymentsForPreviouslyRemovedUnpaidPaymentPeriodsShouldBeAdded()
    {
        _result.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 2 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }

    [Test]
    public void NewPaymentsForPreviouslyUncalculatedPaymentPeriodsShouldBeAdded()
    {
        _result.Payments.Should().Contain(p => p.AcademicYear == 2223 && p.DeliveryPeriod == 3 && p.Amount == _newMonthlyLearningAmount && p.EarningsProfileId == _newEarningsProfileId);
    }
}