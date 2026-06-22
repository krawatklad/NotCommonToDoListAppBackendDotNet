using Application.Common.Enums;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Persistence;

public interface ITaskItemRepository
{
    Task<TaskItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    void Delete(TaskItem taskItem);

    Task<PaginatedList<TaskItem>> GetPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        TaskItemStatus[]? statuses = null,
        TaskItemOrderBy orderBy = TaskItemOrderBy.Title,
        SortDirection sortDirection = SortDirection.Asc,
        CancellationToken cancellationToken = default);

    Task<List<TaskItem>> GetAllAsync(
        Guid userId,
        string? search = null,
        TaskItemStatus[]? statuses = null,
        TaskItemOrderBy orderBy = TaskItemOrderBy.Title,
        SortDirection sortDirection = SortDirection.Asc,
        CancellationToken cancellationToken = default);
}
