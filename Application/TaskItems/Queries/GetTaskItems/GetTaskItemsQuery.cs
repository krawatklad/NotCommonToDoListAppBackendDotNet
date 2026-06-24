using Application.Common.Enums;
using Domain.Enums;

namespace Application.TaskItems.Queries.GetTaskItems;

public record GetTaskItemsQuery(
    Guid UserId,
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    TaskItemStatus[]? Statuses = null,
    TaskItemOrderBy OrderBy = TaskItemOrderBy.Title,
    SortDirection SortDirection = SortDirection.Asc);