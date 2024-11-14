using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ResetSentForPaymentFlagForCollectionPeriod;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Dtos;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests;

[TestFixture]
public class ApprenticeshipEntity_ResetSentForPaymentFlagForCollectionPeriodTests
{
    private ApprenticeshipEntity _sut;
    private Mock<IResetSentForPaymentFlagForCollectionPeriodCommandHandler> _resetSentForPaymentFlagForCollectionPeriodCommandHandler;
    private Fixture _fixture;
    private byte _collectionPeriod;
    private short _collectionYear;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();

        _collectionPeriod = _fixture.Create<byte>();
        _collectionYear = _fixture.Create<short>();

        _resetSentForPaymentFlagForCollectionPeriodCommandHandler = new Mock<IResetSentForPaymentFlagForCollectionPeriodCommandHandler>();
        _sut = new ApprenticeshipEntity(Mock.Of<ICalculateApprenticeshipPaymentsCommandHandler>(), Mock.Of<IProcessUnfundedPaymentsCommandHandler>(), Mock.Of<IRecalculateApprenticeshipPaymentsCommandHandler>(), Mock.Of<ILogger<ApprenticeshipEntity>>(), _resetSentForPaymentFlagForCollectionPeriodCommandHandler.Object);
        _sut.Model = _fixture.Create<ApprenticeshipEntityModel>();

        await _sut.ResetSentForPaymentFlagForCollectionPeriod(new ResetSentForPaymentFlagForCollectionPeriodDto{ CollectionPeriod = _collectionPeriod, CollectionYear = _collectionYear });
    }

    [Test]
    public void ThenSentForPaymentsFlagsForGivenCollectionPeriodShouldBeReset()
    {
        _resetSentForPaymentFlagForCollectionPeriodCommandHandler.Verify(x => x.Process(It.Is<ResetSentForPaymentFlagForCollectionPeriodCommand>(y => y.CollectionPeriod == _collectionPeriod && y.CollectionYear == _collectionYear && y.Model == _sut.Model)));
    }
}