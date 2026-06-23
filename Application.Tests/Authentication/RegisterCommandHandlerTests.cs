using Application.Abstractions;
using Application.Authentication.Commands.Register;
using Application.Authentication.Events;
using Application.Authentication.Events.UserRegistered;
using Application.Authentication.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Persistence;
using Domain.Entities;
using Moq;

namespace Application.Tests.Authentication;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<TimeProvider> _timeProviderMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _messageBusMock = new Mock<IMessageBus>();
        _timeProviderMock = new Mock<TimeProvider>();
        
        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _messageBusMock.Object,
            _timeProviderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowValidationException()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "Password123!");
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "hashed_password",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateUserAndSendEmail()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "Password123!");
        const string hashedPassword = "hashed_password";
        var now = new DateTimeOffset(2026, 6, 18, 12, 0, 0, TimeSpan.Zero);

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);
        _timeProviderMock.Setup(x => x.GetUtcNow())
            .Returns(now);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => 
            u.Email == command.Email && 
            u.FirstName == command.FirstName && 
            u.LastName == command.LastName &&
            u.Password == hashedPassword &&
            u.CreatedAt == now &&
            u.UpdatedAt == now
        ), It.IsAny<CancellationToken>()), Times.Once);
        
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
