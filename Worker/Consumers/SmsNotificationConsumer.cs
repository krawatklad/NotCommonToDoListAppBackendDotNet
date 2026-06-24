using Application.Authentication.Events.UserRegistered;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker.Consumers;

public class SmsNotificationConsumer(
    IBus bus,
    IServiceProvider serviceProvider,
    ILogger<SmsNotificationConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SmsNotificationConsumer starting...");

        await bus.PubSub.SubscribeAsync<UserRegisteredEvent>("worker_sms_notification", async (msg, ct) =>
        {
            logger.LogInformation("Received UserRegisteredEvent: {Email}", msg.Email);
            
            await using var scope = serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<SendSmsOnUserRegisteredEventHandler>();
            
            await handler.HandleAsync(msg, ct);
        }, _ => {}, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}