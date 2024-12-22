using System;
using api.Models;

namespace api.Tests.Builders;

public class TaskItemBuilder
{
    private TaskItem _taskItem;

    public TaskItemBuilder()
    {
        _taskItem = new TaskItem
        {
            Title = Guid.NewGuid().ToString(),
        };
    }

    public TaskItemBuilder WithId(int id)
    {
        _taskItem.Id = id;
        return this;
    }

    public TaskItemBuilder WithTitle(string title)
    {
        _taskItem.Title = title;
        return this;
    }

    public TaskItemBuilder WithDescription(string description)
    {
        _taskItem.Description = description;
        return this;
    }

    public TaskItemBuilder WithStatus(TaskItemStatus status)
    {
        _taskItem.Status = status;
        return this;
    }

    public TaskItem Build()
    {
        return _taskItem;
    }

    public static List<TaskItem> BuildMany(int count)
    {
        var taskItems = new List<TaskItem>();
        for (int i = 0; i < count; i++)
        {
            taskItems.Add(new TaskItemBuilder()
                .WithTitle(Guid.NewGuid().ToString())
                .Build());
        }
        return taskItems;
    }
}

