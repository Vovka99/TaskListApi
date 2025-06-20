namespace TaskListApi.Models;

public class TaskList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> SharedWithUserIds { get; set; } = new();
}
