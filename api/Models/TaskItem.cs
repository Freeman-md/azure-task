using System.ComponentModel.DataAnnotations;

namespace api.Models;

public enum TaskItemStatus {
    Pending,
    InProgress,
    Done
}

public class TaskItem {

    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [AllowedValues(new object[] { TaskItemStatus.Pending, TaskItemStatus.InProgress, TaskItemStatus.Done })]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public List<string> Images { get; set; } = new List<string>();
}