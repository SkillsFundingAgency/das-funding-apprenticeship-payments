using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Command;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateRequiredLevyAmount;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.UnitTests;

public class CalculateRequiredLevyAmountFunction_RunTests
{
    private readonly CalculateRequiredLevyAmountFunction _sut;
    private readonly Mock<ICommandHandler<CalculateRequiredLevyAmountCommand>> _commandHandlerMock = new();
    private readonly Fixture _fixture = new();

    public CalculateRequiredLevyAmountFunction_RunTests()
    {
        _sut = new CalculateRequiredLevyAmountFunction(_commandHandlerMock.Object, Mock.Of<ILogger<CalculateRequiredLevyAmountFunction>>());
    }

    [Test]
    public async Task Run_PublishesCalculateRequiredLevyAmountCommand()
    {
        // Arrange
        var @event = _fixture.Create<FinalisedOnProgammeLearningPaymentEvent>();

        // Act
        await _sut.Run(@event);

        // Assert
        _commandHandlerMock.Verify(_ => _.Handle(It.Is<CalculateRequiredLevyAmountCommand>(c => c.Data == @event)), Times.Once());
    }
}