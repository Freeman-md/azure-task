using System;
using api.Contracts;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace api.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TaskItemRepository(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<TaskItem> Create(TaskItem taskItem)
    {
        if (taskItem == null) throw new ArgumentNullException(nameof(taskItem));

        EntityEntry<TaskItem> createdTaskItem = await _dbContext.TaskItems.AddAsync(taskItem);
        await _dbContext.SaveChangesAsync();

        return createdTaskItem.Entity;
    }

    public async Task Delete(int id)
    {
        TaskItem? itemToDelete = await GetOne(id);

        if (itemToDelete == null) throw new KeyNotFoundException($"TaskItem with ID {id} not found.");

        _dbContext.TaskItems.Remove(itemToDelete);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TaskItem>> GetAll()
    {
        return await _dbContext.TaskItems.ToListAsync();
    }

    public async Task<TaskItem?> GetOne(int id)
    {
        return await _dbContext.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem> Update(int id, Dictionary<string, object> updates)
{
    TaskItem? itemToUpdate = await GetOne(id);
    if (itemToUpdate == null) throw new KeyNotFoundException($"TaskItem with ID {id} not found.");

    foreach (var update in updates)
    {
        var propertyInfo = typeof(TaskItem).GetProperty(update.Key);
        if (propertyInfo != null && propertyInfo.CanWrite)
        {
            propertyInfo.SetValue(itemToUpdate, update.Value);
        }
    }

    await _dbContext.SaveChangesAsync();
    return itemToUpdate;
}
}
