using Application.Abstractions;
using Application.Common.Exceptions;
using Application.Common.Persistence;
using Domain.Entities;

namespace Application.TaskItems.Queries.FindTaskItem;

public class FindTaskItemQueryHandler(
    ITaskItemRepository taskItemRepository) : IQueryHandler<FindTaskItemQuery, TaskItem?>
{
    public async Task<TaskItem?> Handle(FindTaskItemQuery query, CancellationToken cancellationToken = default)
    {
        var taskItem = await taskItemRepository.FindByIdAsync(query.TaskItemId, cancellationToken);
        if (taskItem is null)
        {
            return null;
        }

        if (!taskItem.CreatedById.Equals(query.UserId))
        {
            throw new ForbiddenException("Task item is not owned by current user.");
        }

        return taskItem;
    }
}