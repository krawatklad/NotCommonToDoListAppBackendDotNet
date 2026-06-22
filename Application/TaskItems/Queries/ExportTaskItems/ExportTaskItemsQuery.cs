using Application.Common.Enums;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Enums;

namespace Application.TaskItems.Queries.ExportTaskItems;

public record ExportTaskItemsQuery(
    Guid UserId,
    string? Search = null,
    TaskItemStatus[]? Statuses = null,
    TaskItemOrderBy OrderBy = TaskItemOrderBy.Title,
    SortDirection SortDirection = SortDirection.Asc,
    ExportFormat Format = ExportFormat.Xlsx);
