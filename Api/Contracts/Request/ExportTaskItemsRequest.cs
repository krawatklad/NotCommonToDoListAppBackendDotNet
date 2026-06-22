using Application.Common.Enums;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Enums;

namespace Api.Contracts.Request;

public record ExportTaskItemsRequest(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    TaskItemStatus[]? Statuses = null,
    ExportFormat Format = ExportFormat.Xlsx,
    TaskItemOrderBy OrderBy = TaskItemOrderBy.Title,
    SortDirection OrderByDirection = SortDirection.Asc);
