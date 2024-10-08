using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
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
    private Mock<IApprenticeship> _apprenticeship = null!;
    private List<Earning> _newEarnings = null!;
    private List<Earning> _existingEarnings = null!;
    private List<Payment> _existingPayments = null!;
    private Guid _previousEarningsProfileId = Guid.NewGuid();
    private Guid _newEarningsProfileId = Guid.NewGuid();
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Mock<IDasServiceBusEndpoint> _busEndpoint = null!;
    private Mock<IPaymentsGeneratedEventBuilder> _paymentsGeneratedEventBuilder = null!;
    private Apprenticeship _result;
    private decimal _currentMonthlyLearningAmount;
    private decimal _newMonthlyLearningAmount;
    private Guid _apprenticeshipKey;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();
        _apprenticeship = new Mock<IApprenticeship>();
        _newEarnings = _fixture.CreateMany<Earning>().ToList();
        _apprenticeshipKey = Guid.NewGuid();
        _command = new RecalculateApprenticeshipPaymentsCommand(_apprenticeshipKey, _newEarnings);

        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();

        _busEndpoint = new Mock<IDasServiceBusEndpoint>();
        _paymentsGeneratedEventBuilder = new Mock<IPaymentsGeneratedEventBuilder>();

        _apprenticeshipRepository.Setup(x => x.Get(_apprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);

        _paymentsGeneratedEventBuilder.Setup(x => x.Build(It.IsAny<Apprenticeship>()))
            .Returns(new PaymentsGeneratedEvent());

        _sut = new RecalculateApprenticeshipPaymentsCommandHandler(
            _apprenticeshipRepository.Object,
            _busEndpoint.Object,
            _paymentsGeneratedEventBuilder.Object,
            Mock.Of<ILogger<CalculateApprenticeshipPaymentsCommandHandler>>());
        await _sut.Recalculate(_command);
    }

    [Test]
    public void ThenTheEarningsAreReplaced()
    {
        _apprenticeship.Verify(x => x.ClearEarnings(), Times.Once);
        _apprenticeship.Verify(
            x => x.AddEarning(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<decimal>(), It.IsAny<short>(),
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(_newEarnings.Count));

        foreach (var expectedEarning in _newEarnings)
        {
            _apprenticeship.Verify(
                x => x.AddEarning(expectedEarning.AcademicYear, expectedEarning.DeliveryPeriod, expectedEarning.Amount,
                    expectedEarning.CollectionYear, expectedEarning.CollectionMonth, expectedEarning.FundingLineType,
                    _newEarningsProfileId), Times.Once);
        }
    }

    [Test]
    public void PaymentsAreRecalculated()
    {
        _apprenticeship.Verify(x => x.RecalculatePayments(It.IsAny<DateTime>()), Times.Once);
    }

    [Test]
    public void ApprenticeshipIsUpdated()
    {
        _apprenticeshipRepository.Verify(x => x.Update(_apprenticeship.Object), Times.Once);
    }
}