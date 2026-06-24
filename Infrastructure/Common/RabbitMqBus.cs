using Application.Abstractions;
using EasyNetQ;

namespace Infrastructure.Common;

public class RabbitMqBus(IBus bus) : IMessageBus
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        return bus.PubSub.PublishAsync(message, cancellationToken);
    }
}