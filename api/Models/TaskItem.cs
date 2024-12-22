namespace api.Models;

public enum TaskItemStatus {
    Pending,
    InProgress,
    Done
}

public class TaskItem {
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public List<string> Images { get; set; } = new List<string>();
}