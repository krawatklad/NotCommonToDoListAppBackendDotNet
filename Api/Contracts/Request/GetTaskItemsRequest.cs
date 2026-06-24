using Application.Common.Enums;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Enums;

namespace Api.Contracts.Request;

public record GetTaskItemsRequest(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    TaskItemStatus[]? Statuses = null,
    TaskItemOrderBy OrderBy = TaskItemOrderBy.Title,
    SortDirection OrderByDirection = SortDirection.Asc);