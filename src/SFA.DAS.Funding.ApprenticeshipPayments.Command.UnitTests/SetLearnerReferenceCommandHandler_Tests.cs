using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.SetLearnerReference;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests;

public class SetLearnerReferenceCommandHandler_Tests
{
    private SetLearnerReferenceCommand _command = null!;
    private Fixture _fixture = null!;
    private SetLearnerReferenceCommandHandler _sut = null!;
    private Mock<IApprenticeship> _apprenticeship = null!;
    private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
    private Guid _apprenticeshipKey;
    
    [SetUp]
    public async Task SetUp()
    {
        _fixture = new Fixture();
        _apprenticeship = new Mock<IApprenticeship>();
        _apprenticeshipKey = Guid.NewGuid();
        _command = new SetLearnerReferenceCommand(_apprenticeshipKey, _fixture.Create<string>());
        
        _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
        _apprenticeshipRepository.Setup(x => x.Get(_apprenticeshipKey)).ReturnsAsync(_apprenticeship.Object);

        _sut = new SetLearnerReferenceCommandHandler(
            _apprenticeshipRepository.Object,
            Mock.Of<ILogger<CalculateApprenticeshipPaymentsCommandHandler>>());
        await _sut.Set(_command);
    }

    [Test]
    public void ThenTheLearnerReferenceIsSet()
    {
        _apprenticeship.Verify(x => x.SetLearnerReference(_command.LearnerReference), Times.Once);
    }
}