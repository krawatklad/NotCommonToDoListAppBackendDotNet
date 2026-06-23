using Application.Authentication.Events;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker.Consumers;

public class SmsNotificationConsumer(
    IBus bus,
    ILogger<SmsNotificationConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SmsNotificationConsumer starting...");

        await bus.PubSub.SubscribeAsync<UserRegisteredEvent>("worker_sms_notification", (msg, ct) =>
        {
            logger.LogInformation("SMS FEATURE TODO: In the future, an SMS will be sent to user with name {Name}", msg.FirstName);
            
            return Task.CompletedTask;
        }, _ => {}, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
