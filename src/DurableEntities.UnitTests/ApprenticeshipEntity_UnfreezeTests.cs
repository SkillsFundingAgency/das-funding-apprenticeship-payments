using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Types;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ResetSentForPaymentFlagForCollectionPeriod;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests;

[TestFixture]
public class ApprenticeshipEntity_UnfreezeTests
{
    private ApprenticeshipEntity _sut;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();

        _sut = new ApprenticeshipEntity(Mock.Of<ICalculateApprenticeshipPaymentsCommandHandler>(), Mock.Of<IProcessUnfundedPaymentsCommandHandler>(), Mock.Of<IRecalculateApprenticeshipPaymentsCommandHandler>(), Mock.Of<ILogger<ApprenticeshipEntity>>(), Mock.Of<IResetSentForPaymentFlagForCollectionPeriodCommandHandler>());
        _sut.Model = _fixture.Create<ApprenticeshipEntityModel>();
        _sut.Model.PaymentsFrozen = true;
        
        _sut.HandlePaymentsUnfrozenEvent(new PaymentsUnfrozenEvent { ApprenticeshipKey = _sut.Model.ApprenticeshipKey });
    }

    [Test]
    public void ThenPaymentsShouldBeMarkedUnfrozen()
    {
        _sut.Model.PaymentsFrozen.Should().BeFalse();
    }
}