namespace Domain.Entities;

public class User
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
    
    public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
}
