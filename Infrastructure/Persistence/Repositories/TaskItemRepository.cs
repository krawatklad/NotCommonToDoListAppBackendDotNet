using Application.Common;
using Application.Common.Enums;
using Application.Common.Extensions;
using Application.Common.Persistence;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TaskItemRepository(AppDbContext dbContext) : ITaskItemRepository
{
    public async Task<TaskItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.TaskItems.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        await dbContext.TaskItems.AddAsync(taskItem, cancellationToken);
    }

    public void Delete(TaskItem taskItem)
    {
        dbContext.TaskItems.Remove(taskItem);
    }

    public async Task<PaginatedList<TaskItem>> GetPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        TaskItemStatus[]? statuses = null,
        TaskItemOrderBy orderBy = TaskItemOrderBy.Title,
        SortDirection sortDirection = SortDirection.Asc,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.TaskItems
            .Where(t => t.CreatedById == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(t => 
                t.Title.ToLower().Contains(lowerSearch) ||
                (t.Description != null && t.Description.ToLower().Contains(lowerSearch)));
        }

        if (statuses is { Length: > 0 })
        {
            query = query.Where(t => statuses.Contains(t.Status));
        }

        query = (orderBy, sortDirection) switch
        {
            (TaskItemOrderBy.Title, SortDirection.Desc) => query.OrderByDescending(t => t.Title),
            (TaskItemOrderBy.Title, _) => query.OrderBy(t => t.Title),
            (TaskItemOrderBy.CreatedAt, SortDirection.Desc) => query.OrderByDescending(t => t.CreatedAt),
            (TaskItemOrderBy.CreatedAt, _) => query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Paginate(pageNumber, pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TaskItem>(items, pageNumber, totalCount, pageSize);
    }

    public async Task<List<TaskItem>> GetAllAsync(
        Guid userId,
        string? search = null,
        TaskItemStatus[]? statuses = null,
        TaskItemOrderBy orderBy = TaskItemOrderBy.Title,
        SortDirection sortDirection = SortDirection.Asc,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.TaskItems
            .Where(t => t.CreatedById == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(t => 
                t.Title.ToLower().Contains(lowerSearch) ||
                (t.Description != null && t.Description.ToLower().Contains(lowerSearch)));
        }

        if (statuses is { Length: > 0 })
        {
            query = query.Where(t => statuses.Contains(t.Status));
        }

        query = (orderBy, sortDirection) switch
        {
            (TaskItemOrderBy.Title, SortDirection.Desc) => query.OrderByDescending(t => t.Title),
            (TaskItemOrderBy.Title, _) => query.OrderBy(t => t.Title),
            (TaskItemOrderBy.CreatedAt, SortDirection.Desc) => query.OrderByDescending(t => t.CreatedAt),
            (TaskItemOrderBy.CreatedAt, _) => query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        return await query.ToListAsync(cancellationToken);
    }
}