using Application.Abstractions;
using Application.Authentication.Events.UserRegistered;
using Application.Authentication.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Persistence;
using Domain.Entities;

namespace Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IMessageBus messageBus,
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
        
        await messageBus.PublishAsync(new UserRegisteredEvent(user.Email, user.FirstName), cancellationToken);
        
        return user.Id;
    }
}