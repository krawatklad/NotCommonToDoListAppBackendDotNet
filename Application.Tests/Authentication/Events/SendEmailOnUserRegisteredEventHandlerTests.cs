using Application.Authentication.Events.UserRegistered;
using Application.Common.Interfaces;
using Moq;

namespace Application.Tests.Authentication.Events;

public class SendEmailOnUserRegisteredEventHandlerTests
{
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly SendEmailOnUserRegisteredEventHandler _handler;

    public SendEmailOnUserRegisteredEventHandlerTests()
    {
        _emailSenderMock = new Mock<IEmailSender>();
        _handler = new SendEmailOnUserRegisteredEventHandler(_emailSenderMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendEmail_WhenUserRegistered()
    {
        // Arrange
        var @event = new UserRegisteredEvent("test@example.com", "John");

        // Act
        await _handler.HandleAsync(@event);

        // Assert
        _emailSenderMock.Verify(
            x => x.SendEmailAsync(
                @event.Email,
                "Welcome to NotCommonToDoListApp!",
                $"Hi {@event.FirstName}, thank you for registering!"),
            Times.Once);
    }
}