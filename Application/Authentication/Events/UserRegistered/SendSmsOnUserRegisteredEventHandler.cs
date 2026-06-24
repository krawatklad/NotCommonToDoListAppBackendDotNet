using Microsoft.Extensions.Logging;

namespace Application.Authentication.Events.UserRegistered;

public class SendSmsOnUserRegisteredEventHandler(ILogger<SendSmsOnUserRegisteredEventHandler> logger)
{
    public Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "SMS FEATURE TODO: In the future, an SMS welcome message will be sent to user with name {Name}",
            @event.FirstName);
            
        return Task.CompletedTask;
    }
}