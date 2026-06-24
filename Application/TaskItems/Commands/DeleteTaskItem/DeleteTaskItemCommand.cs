namespace Application.TaskItems.Commands.DeleteTaskItem;

public record DeleteTaskItemCommand(Guid TaskItemId, Guid UserId);