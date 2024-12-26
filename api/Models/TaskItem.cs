using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models;

public enum TaskItemStatus
{
    Pending,
    InProgress,
    Completed
}

public class TaskItem
{

    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [Base64String(ErrorMessage = "Title must be a base64 string.")]
    [StringLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
    public string? Description { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Please enter a valid date in the format YYYY-MM-DD.")]
    public DateTime DueDate { get; set; } = DateTime.Now;


    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public List<string> Images { get; set; } = new List<string>();
}