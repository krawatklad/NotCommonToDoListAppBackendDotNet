using Application.Abstractions;
using Application.Authentication.DTOs;
using Application.Authentication.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Persistence;

namespace Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository) : IQueryHandler<LoginQuery, LoginResult>
{
    public async Task<LoginResult> Handle(LoginQuery query, CancellationToken cancellationToken = default)
    {
        if(await userRepository.GetUserByEmailAsync(query.Email, cancellationToken) is not { } user ||
           !passwordHasher.VerifyPassword(query.Password, user.Password))
        {
            throw new ValidationException("Invalid credentials!");
        }

        return new LoginResult(
            User: user,
            Token: jwtTokenGenerator.GenerateToken(user: user)
        );
    }
}