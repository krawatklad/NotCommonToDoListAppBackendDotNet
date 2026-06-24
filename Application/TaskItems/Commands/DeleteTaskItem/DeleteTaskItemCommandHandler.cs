using Application.Abstractions;
using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Persistence;

namespace Application.TaskItems.Commands.DeleteTaskItem;

public class DeleteTaskItemCommandHandler(
    ITaskItemRepository taskItemRepository) : ICommandHandler<DeleteTaskItemCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken = default)
    {
        var taskItem = await taskItemRepository.FindByIdAsync(command.TaskItemId, cancellationToken) 
                       ?? throw new NotFoundException($"Task item with id {command.TaskItemId} not found.");
        
        if(!taskItem.CreatedById.Equals(command.UserId))
        {
            throw new ForbiddenException("You are not authorized to delete this task item.");
        }

        taskItemRepository.Delete(taskItem);

        return Unit.Value;
    }
}