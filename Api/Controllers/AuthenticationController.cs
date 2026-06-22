using Api.Contracts.Request;
using Api.Contracts.Response;
using Application.Abstractions;
using Application.Authentication.Commands.Register;
using Application.Authentication.DTOs;
using Application.Authentication.Queries.Login;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(
    ICommandHandler<RegisterCommand, Guid> registerHandler,
    IQueryHandler<LoginQuery, LoginResult> loginQuery) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Email: request.Email,
            Password: request.Password);
        var result = await registerHandler.Handle(command);
        
        return Ok(new RegisterResponse(Id: result));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var query = new LoginQuery(Email: request.Email, Password: request.Password);
        var loginResult = await loginQuery.Handle(query);
        
        return Ok(new LoginResponse(
            Id: loginResult.User.Id,
            Email: loginResult.User.Email,
            FirstName: loginResult.User.FirstName,
            LastName: loginResult.User.LastName,
            Token: loginResult.Token
        ));
    }
}