namespace Api.Contracts.Response;

public record PagedTaskItemsResponse(
    IReadOnlyList<TaskItemResponse> Items,
    int Page,
    int TotalPages,
    int Count,
    bool HasPreviousPage,
    bool HasNextPage);
