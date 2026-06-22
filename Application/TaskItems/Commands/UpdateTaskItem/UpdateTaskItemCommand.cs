using Domain.Enums;

namespace Application.TaskItems.Commands.UpdateTaskItem;

public record UpdateTaskItemCommand(
    Guid TaskItemId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset Deadline,
    Guid UserId);
