using Application.Abstractions;
using Application.Common;
using Application.Common.Persistence;
using Domain.Entities;

namespace Application.TaskItems.Queries.GetTaskItems;

public class GetTaskItemsQueryHandler(ITaskItemRepository taskItemRepository) 
    : IQueryHandler<GetTaskItemsQuery, PaginatedList<TaskItem>>
{
    public async Task<PaginatedList<TaskItem>> Handle(GetTaskItemsQuery query, CancellationToken cancellationToken = default)
    {
        return await taskItemRepository.GetPagedAsync(
            query.UserId,
            query.PageNumber,
            query.PageSize,
            query.Search,
            query.Statuses,
            query.OrderBy,
            query.SortDirection,
            cancellationToken);
    }
}