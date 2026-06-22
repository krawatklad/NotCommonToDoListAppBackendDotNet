using Domain.Enums;

namespace Api.Contracts.Request;

public record UpdateTaskItemRequest(
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset Deadline
);