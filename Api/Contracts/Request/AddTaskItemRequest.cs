using Domain.Enums;

namespace Api.Contracts.Request;

public record AddTaskItemRequest(
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset Deadline
);