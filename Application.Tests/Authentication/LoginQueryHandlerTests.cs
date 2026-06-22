using Application.Authentication.Interfaces;
using Application.Authentication.Queries.Login;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Persistence;
using Domain.Entities;
using Moq;

namespace Application.Tests.Authentication;

public class LoginQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsValidationException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var hasherMock = new Mock<IPasswordHasher>();
        var tokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        var handler = new LoginQueryHandler(tokenGeneratorMock.Object, hasherMock.Object, repositoryMock.Object);
        var query = new LoginQuery(Email: "missing@test.com", Password: "secret");

        // Act
        var action = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task Handle_WhenPasswordInvalid_ThrowsValidationException()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            Password = "stored-hash",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasherMock = new Mock<IPasswordHasher>();
        hasherMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var tokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        var handler = new LoginQueryHandler(tokenGeneratorMock.Object, hasherMock.Object, repositoryMock.Object);
        var query = new LoginQuery(Email: "john@doe.com", Password: "wrong");

        // Act
        var action = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsUserAndToken()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Anna",
            LastName = "Nowak",
            Email = "anna@nowak.com",
            Password = "stored-hash",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasherMock = new Mock<IPasswordHasher>();
        hasherMock.Setup(h => h.VerifyPassword("secret", "stored-hash"))
            .Returns(true);

        var tokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        tokenGeneratorMock.Setup(t => t.GenerateToken(user))
            .Returns("jwt-token");

        var handler = new LoginQueryHandler(tokenGeneratorMock.Object, hasherMock.Object, repositoryMock.Object);
        var query = new LoginQuery(Email: "anna@nowak.com", Password: "secret");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(user, result.User);
        Assert.Equal("jwt-token", result.Token);
    }
}
