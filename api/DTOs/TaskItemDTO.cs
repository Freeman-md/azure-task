using api.Models;

namespace api.DTOs;

public class TaskItemDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DueDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();

    public static TaskItemDTO FromEntity(TaskItem taskItem)
    {
        return new TaskItemDTO
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            DueDate = taskItem.DueDate.ToShortDateString(),
            Status = taskItem.Status.ToString(),            
            Images = taskItem.Images
        };

    }
}
