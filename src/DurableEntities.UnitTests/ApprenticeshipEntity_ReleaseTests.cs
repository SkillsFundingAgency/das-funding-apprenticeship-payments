using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests;

[TestFixture]
public class ApprenticeshipEntity_ReleaseTests
{
    private ApprenticeshipEntity _sut;
    private Mock<IProcessUnfundedPaymentsCommandHandler> _processUnfundedPaymentsCommandHandler;
    private Fixture _fixture;
    private byte _collectionPeriod;
    private short _collectionYear;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();

        _collectionPeriod = _fixture.Create<byte>();
        _collectionYear = _fixture.Create<short>();

        _processUnfundedPaymentsCommandHandler = new Mock<IProcessUnfundedPaymentsCommandHandler>();
        _sut = new ApprenticeshipEntity(Mock.Of<ICalculateApprenticeshipPaymentsCommandHandler>(), _processUnfundedPaymentsCommandHandler.Object, Mock.Of<ILogger<ApprenticeshipEntity>>());
        _sut.Model = _fixture.Create<ApprenticeshipEntityModel>();

        await _sut.ReleasePaymentsForCollectionPeriod(new ReleasePaymentsCommand(){CollectionPeriod = _collectionPeriod, CollectionYear = _collectionYear});
    }

    [Test]
    public void ThenPaymentsShouldBeReleased()
    {
        _processUnfundedPaymentsCommandHandler.Verify(x => x.Process(It.Is<ProcessUnfundedPaymentsCommand>(y => y.CollectionPeriod == _collectionPeriod && y.CollectionYear == _collectionYear && y.Model == _sut.Model)));
    }
}