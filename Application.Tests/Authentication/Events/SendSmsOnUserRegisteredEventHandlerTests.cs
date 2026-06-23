using Application.Authentication.Events.UserRegistered;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Authentication.Events;

public class SendSmsOnUserRegisteredEventHandlerTests
{
    private readonly Mock<ILogger<SendSmsOnUserRegisteredEventHandler>> _loggerMock;
    private readonly SendSmsOnUserRegisteredEventHandler _handler;

    public SendSmsOnUserRegisteredEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<SendSmsOnUserRegisteredEventHandler>>();
        _handler = new SendSmsOnUserRegisteredEventHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogInformation_WhenUserRegistered()
    {
        // Arrange
        var @event = new UserRegisteredEvent("test@example.com", "John");

        // Act
        await _handler.HandleAsync(@event);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"SMS welcome message will be sent to user with name {@event.FirstName}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
