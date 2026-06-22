using Application.Abstractions;
using Application.Common.Interfaces;

namespace Application.Common.Decorators;

internal sealed class TransactionalCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler, IUnitOfWork unitOfWork) : ICommandHandler<TCommand, TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await innerHandler.Handle(command, cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}