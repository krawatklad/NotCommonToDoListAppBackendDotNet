using Application.Abstractions;
using Application.Common.Exceptions;
using Application.Common.Persistence;
using Domain.Entities;

namespace Application.TaskItems.Commands.UpdateTaskItem;

public class UpdateTaskItemCommandHandler(
    ITaskItemRepository taskItemRepository,
    TimeProvider timeProvider) : ICommandHandler<UpdateTaskItemCommand, TaskItem>
{
    public async Task<TaskItem> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken = default)
    {
        var taskItem = await taskItemRepository.FindByIdAsync(command.TaskItemId, cancellationToken)
            ?? throw new NotFoundException($"Task item with id {command.TaskItemId} not found.");
        
        var now = timeProvider.GetUtcNow();
        taskItem.Update(
            title: command.Title,
            description: command.Description,
            status: command.Status,
            deadline: command.Deadline,
            userId: command.UserId,
            updatedAt: now);

        return taskItem;
    }
}
