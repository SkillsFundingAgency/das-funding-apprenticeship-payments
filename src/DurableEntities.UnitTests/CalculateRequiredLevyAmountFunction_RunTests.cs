using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;
using System.Threading.Tasks;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests;

public class CalculateRequiredLevyAmountFunction_RunTests
{
    private readonly CalculateRequiredLevyAmountFunction _sut;
    private readonly Mock<ICalculateRequiredLevyAmountCommandHandler> _commandHandlerMock = new();
    private readonly Fixture _fixture = new();

    public CalculateRequiredLevyAmountFunction_RunTests()
    {
        _sut = new CalculateRequiredLevyAmountFunction(_commandHandlerMock.Object);
    }

    [Test]
    [Ignore("temp")]
    public async Task Run_SendsCalculateRequiredLevyAmountCommand()
    {
        // Arrange
        var @event = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

        // Act
        await _sut.Run(@event, Mock.Of<ILogger>());

        // Assert
        _commandHandlerMock.Verify(_ => _.Process(It.Is<CalculateRequiredLevyAmountCommand>(c => c.Data == @event)), Times.Once());
    }
}