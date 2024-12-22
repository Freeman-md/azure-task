using System;
using api.Contracts;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace api.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ILogger<TaskItemRepository> _logger;
    private readonly ApplicationDbContext _dbContext;

    public TaskItemRepository(ILogger<TaskItemRepository> logger, ApplicationDbContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<TaskItem> Create(TaskItem taskItem)
    {
        if (taskItem == null) throw new ArgumentNullException(nameof(taskItem));

        EntityEntry<TaskItem> createdTaskItem = await _dbContext.TaskItems.AddAsync(taskItem);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"TaskItem created: {createdTaskItem.Entity.Id}");

        return createdTaskItem.Entity;
    }

    public async Task Delete(int id)
    {
        TaskItem? itemToDelete = await GetOne(id);

        if (itemToDelete == null) throw new KeyNotFoundException($"TaskItem with ID {id} not found.");

        _dbContext.TaskItems.Remove(itemToDelete);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"TaskItem deleted: {id}");
    }

    public async Task<IReadOnlyList<TaskItem>> GetAll()
    {
        return await _dbContext.TaskItems.ToListAsync();
    }

    public async Task<TaskItem?> GetOne(int id)
    {
        return await _dbContext.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem> Update(TaskItem taskItem, int id)
    {
        TaskItem? itemToUpdate = await GetOne(id);
        if (itemToUpdate == null) throw new KeyNotFoundException($"TaskItem with ID {id} not found.");

        itemToUpdate.Title = taskItem.Title;
        itemToUpdate.Description = taskItem.Description;
        itemToUpdate.DueDate = taskItem.DueDate;
        itemToUpdate.Status = taskItem.Status;
        itemToUpdate.Images = taskItem.Images;
        await _dbContext.SaveChangesAsync();

        return itemToUpdate;
    }
}
