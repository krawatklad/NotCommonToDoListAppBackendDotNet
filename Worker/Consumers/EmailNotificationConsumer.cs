using Application.Authentication.Events.UserRegistered;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker.Consumers;

public class EmailNotificationConsumer(
    IBus bus,
    IServiceProvider serviceProvider,
    ILogger<EmailNotificationConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("EmailNotificationConsumer starting...");

        await bus.PubSub.SubscribeAsync<UserRegisteredEvent>(
        "worker_email_notification",
        async (msg, ct) =>
        {
            logger.LogInformation("Received UserRegisteredEvent: {Email}", msg.Email);
            
            await using var scope = serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<SendEmailOnUserRegisteredEventHandler>();
            
            await handler.HandleAsync(msg, ct);
        }, _ => {}, stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
