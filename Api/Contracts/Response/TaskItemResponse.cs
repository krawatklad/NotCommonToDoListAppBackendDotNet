using Domain.Enums;

namespace Api.Contracts.Response;

public record TaskItemResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset Deadline,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

