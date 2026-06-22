using Domain.Enums;

namespace Application.TaskItems.Commands.AddTaskItem;

public record AddTaskItemCommand(
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTimeOffset Deadline,
    Guid UserId);
