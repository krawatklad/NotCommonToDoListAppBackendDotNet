using Application.Abstractions;
using Application.Common.Persistence;
using Domain.Entities;

namespace Application.TaskItems.Commands.AddTaskItem;

public class AddTaskItemCommandHandler(
    ITaskItemRepository taskItemRepository,
    TimeProvider timeProvider) : ICommandHandler<AddTaskItemCommand, Guid>
{
    public async Task<Guid> Handle(AddTaskItemCommand command, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();
        var taskItem = new TaskItem
        (
            title: command.Title,
            description: command.Description,
            status: command.Status,
            deadline: command.Deadline,
            createdById: command.UserId,
            createdAt: now
        );

        await taskItemRepository.AddAsync(taskItem, cancellationToken);
        
        return taskItem.Id;
    }
}
