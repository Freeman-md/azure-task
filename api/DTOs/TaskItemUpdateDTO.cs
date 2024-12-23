using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTOs;

public class TaskItemUpdateDTO
{
    [StringLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
    public string? Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
    public TaskItemStatus? Status { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Please enter a valid date in the format YYYY-MM-DD.")]
    public DateTime? DueDate { get; set; }
}
