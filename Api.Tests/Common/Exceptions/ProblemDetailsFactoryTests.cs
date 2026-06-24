using Api.Common.Exceptions;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Common.Exceptions;

public class ProblemDetailsFactoryTests
{
    private readonly ProblemDetailsFactory _factory = new();

    [Fact]
    public void Create_WhenValidationException_ShouldReturn400()
    {
        // Arrange
        const string message = "Validation failed";
        var exception = new ValidationException(message);

        // Act
        var result = _factory.Create(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
        Assert.Equal(nameof(ValidationException), result.Title);
        Assert.Equal(message, result.Detail);
    }

    [Fact]
    public void Create_WhenNotFoundException_ShouldReturn404()
    {
        // Arrange
        const string message = "Not found";
        var exception = new NotFoundException(message);

        // Act
        var result = _factory.Create(exception);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, result.Status);
        Assert.Equal(nameof(NotFoundException), result.Title);
        Assert.Equal(message, result.Detail);
    }

    [Fact]
    public void Create_WhenForbiddenException_ShouldReturn403()
    {
        // Arrange
        const string message = "Forbidden";
        var exception = new ForbiddenException(message);

        // Act
        var result = _factory.Create(exception);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, result.Status);
        Assert.Equal(nameof(ForbiddenException), result.Title);
        Assert.Equal(message, result.Detail);
    }

    [Fact]
    public void Create_WhenUnhandledException_ShouldReturn500()
    {
        // Arrange
        const string message = "Something went wrong";
        var exception = new Exception(message);

        // Act
        var result = _factory.Create(exception);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, result.Status);
        Assert.Equal("Internal Server Error", result.Title);
        Assert.Equal(message, result.Detail);
        Assert.Equal(nameof(Exception), result.Type);
    }
}