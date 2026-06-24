using Application.Common.Interfaces;

namespace Application.Authentication.Events.UserRegistered;

public class SendEmailOnUserRegisteredEventHandler(IEmailSender emailSender)
{
    public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        await emailSender.SendEmailAsync(
            @event.Email, 
            "Welcome to NotCommonToDoListApp!", 
            $"Hi {@event.FirstName}, thank you for registering!");
    }
}