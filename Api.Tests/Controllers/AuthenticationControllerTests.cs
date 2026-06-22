using Api.Contracts.Request;
using Api.Contracts.Response;
using Api.Controllers;
using Application.Abstractions;
using Application.Authentication.Commands.Register;
using Application.Authentication.DTOs;
using Application.Authentication.Queries.Login;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<ICommandHandler<RegisterCommand, Guid>> _registerHandlerMock;
    private readonly Mock<IQueryHandler<LoginQuery, LoginResult>> _loginQueryMock;
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _registerHandlerMock = new Mock<ICommandHandler<RegisterCommand, Guid>>();
        _loginQueryMock = new Mock<IQueryHandler<LoginQuery, LoginResult>>();
        _controller = new AuthenticationController(_registerHandlerMock.Object, _loginQueryMock.Object);
    }

    [Fact]
    public async Task Register_WhenValidRequest_ShouldReturnOkWithId()
    {
        // Arrange
        var request = new RegisterRequest(
            "John", 
            "Doe", 
            "john@example.com", 
            "Password123!"
        );
        var userId = Guid.NewGuid();
        _registerHandlerMock.Setup(x => 
                x.Handle(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<RegisterResponse>(okResult.Value);
        Assert.Equal(userId, response.Id);
        _registerHandlerMock.Verify(x => 
            x.Handle(It.Is<RegisterCommand>(c => 
            c.FirstName == request.FirstName &&
            c.LastName == request.LastName &&
            c.Email == request.Email &&
            c.Password == request.Password), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_WhenValidRequest_ShouldReturnOkWithLoginResponse()
    {
        // Arrange
        var request = new LoginRequest("john@example.com", "Password123!");
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "john@example.com", 
            FirstName = "John", 
            LastName = "Doe",
            Password = "hashed_password",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var loginResult = new LoginResult(user, "token");
        
        _loginQueryMock.Setup(x => 
                x.Handle(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(user.Id, response.Id);
        Assert.Equal(user.Email, response.Email);
        Assert.Equal("token", response.Token);
        _loginQueryMock.Verify(x => x.Handle(It.Is<LoginQuery>(q => 
            q.Email == request.Email &&
            q.Password == request.Password), It.IsAny<CancellationToken>()), Times.Once);
    }
}
