using AutoFixture;
using Moq;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.UnfreezePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.DataAccess.Repositories;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.UnitTests
{
    public class UnfreezePaymentsCommandHandlerTests
    {
        private Fixture _fixture = null!;
        private Mock<IApprenticeship> _apprenticeshipMock = null!;
        private UnfreezePaymentsCommand _command = null!;
        private Mock<IApprenticeshipRepository> _repositoryMock = null!;
        private UnfreezePaymentsCommandHandler _sut = null!;

        [SetUp]
        public async Task Setup()
        {
            _fixture = new Fixture();
            _command = _fixture.Create<UnfreezePaymentsCommand>();

            _apprenticeshipMock = new Mock<IApprenticeship>();
            _repositoryMock = new Mock<IApprenticeshipRepository>();

            // Setup repository mock behavior
            _repositoryMock
                .Setup(x => x.Get(_command.ApprenticeshipKey))
                .ReturnsAsync(_apprenticeshipMock.Object);

            _sut = new UnfreezePaymentsCommandHandler(_repositoryMock.Object);

            // Execute handler
            await _sut.Handle(_command);
        }

        [Test]
        public void ThenPaymentsAreUnfrozen()
        {
            _apprenticeshipMock.Verify(x => x.UnfreezePayments(), Times.Once);
        }

        [Test]
        public void ThenApprenticeshipIsUpdated()
        {
            _repositoryMock.Verify(x => x.Update(_apprenticeshipMock.Object), Times.Once);
        }
    }
}