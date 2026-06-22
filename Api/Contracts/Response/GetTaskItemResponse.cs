using Domain.Enums;

namespace Api.Contracts.Response;

public record GetTaskItemResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset Deadline,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

