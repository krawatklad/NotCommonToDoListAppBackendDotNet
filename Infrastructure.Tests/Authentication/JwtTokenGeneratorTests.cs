using System.IdentityModel.Tokens.Jwt;
using Application.Common.Configurations;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace Infrastructure.Tests.Authentication;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateToken_ReturnsValidJwt_WithExpectedClaimsAndExpiry()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetUtcNow(new DateTime(2026, 03, 13, 12, 45, 00));
        var options = Options.Create(new JwtOptions
        {
            SigningKey = "super_secret_signing_key_1234567",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ExpireMinutes = 60
        });
        var expectedExpiry = timeProvider.GetUtcNow().AddMinutes(options.Value.ExpireMinutes).UtcDateTime;
        var handler = new JwtSecurityTokenHandler();
        const string firstName = "John";
        const string lastName = "Doe";
        var userId = Guid.Parse("019ce705-d789-7e8e-b00a-daf44173cf0e");
        var user = new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = "test@test.com",
            Password = "test",
            CreatedAt = default,
            UpdatedAt = default,
        };
        var generator = new JwtTokenGenerator(options, timeProvider);

        // Act
        var token = generator.GenerateToken(user: user);

        // Assert
        var jwt = handler.ReadJwtToken(token);
        Assert.NotNull(jwt);
        Assert.Equal(firstName, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value);
        Assert.Equal(lastName, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value);
        Assert.Equal(userId.ToString(), jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value);
        Assert.Equal(expectedExpiry, jwt.ValidTo);
        Assert.Equal(options.Value.Issuer, jwt.Issuer);
        Assert.Equal(options.Value.Audience, jwt.Audiences.FirstOrDefault());
    }
}