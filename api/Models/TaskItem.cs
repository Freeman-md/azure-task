using System.ComponentModel.DataAnnotations;

namespace api.Models;

public enum TaskItemStatus {
    Pending,
    InProgress,
    Done
}

public class TaskItem {

    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
    public required string Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
    public string? Description { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Please enter a valid date in the format YYYY-MM-DD.")]
    public DateTime DueDate { get; set; }


    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public List<string> Images { get; set; } = new List<string>();
}