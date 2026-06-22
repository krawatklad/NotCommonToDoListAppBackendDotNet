using Application.Abstractions;
using Application.Authentication.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Persistence;
using Domain.Entities;

namespace Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailSender emailSender,
    TimeProvider timeProvider) : ICommandHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        if(await userRepository.GetUserByEmailAsync(command.Email, cancellationToken) is not null)
        {
            throw new ValidationException("User with this email already exists!");
        }
        
        var now = timeProvider.GetUtcNow();
        var user = new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Password = passwordHasher.HashPassword(command.Password),
            CreatedAt = now,
            UpdatedAt = now,
        };
        
        await userRepository.AddAsync(user, cancellationToken);
        
        // TODO in the future add this to async queue
        await emailSender.SendEmailAsync(
            email: user.Email, 
            subject: "Welcome to our service!", 
            message: $"Hello {user.FirstName}, thank you for registering!"
            );
        
        return user.Id;
    }
}
