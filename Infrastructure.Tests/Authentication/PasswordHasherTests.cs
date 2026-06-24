using Infrastructure.Authentication;

namespace Infrastructure.Tests.Authentication;

public class PasswordHasherTests
{
    [Fact]
    public void VerifyPassword_WhenPasswordMatches_ReturnsTrue()
    {
        // Arrange
        var hasher = new PasswordHasher();
        const string password = "super-secret";
        var hash = hasher.HashPassword(password);

        // Act
        var result = hasher.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordDoesNotMatch_ReturnsFalse()
    {
        // Arrange
        var hasher = new PasswordHasher();
        const string password = "super-secret";
        var hash = hasher.HashPassword(password);

        // Act
        var result = hasher.VerifyPassword("wrong", hash);

        // Assert
        Assert.False(result);
    }
}