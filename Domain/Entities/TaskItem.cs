using Domain.Enums;

namespace Domain.Entities;

public class TaskItem
{
    public TaskItem(
        string title,
        string? description,
        TaskItemStatus status,
        DateTimeOffset deadline,
        Guid createdById,
        DateTimeOffset createdAt)
    {
        Title = title;
        Description = description;
        Status = status;
        Deadline = deadline.ToUniversalTime();
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        CreatedById = createdById;
        UpdatedById = createdById;
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();

    public string Title
    {
        get;
        private set => field = string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value[1..];
    }

    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public DateTimeOffset Deadline { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid CreatedById { get; init; }
    public Guid UpdatedById { get; private set; }
    public User CreatedBy { get; set; } = null!;
    public User UpdatedBy { get; set; } = null!;

    public void Update(
        string title,
        string? description,
        TaskItemStatus status,
        DateTimeOffset deadline,
        Guid userId,
        DateTimeOffset updatedAt)
    {
        Title = title;
        Description = description;
        Status = status;
        Deadline = deadline.ToUniversalTime();
        UpdatedAt = updatedAt;
        UpdatedById = userId;
    }
}
