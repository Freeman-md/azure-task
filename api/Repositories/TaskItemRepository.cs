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

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<TaskItem>> GetAll()
    {
        return await _dbContext.TaskItems.ToListAsync();
    }

    public async Task<TaskItem?> GetOne(int id)
    {
        return await _dbContext.TaskItems.FindAsync(id);
    }

    public Task<TaskItem> Update(TaskItem taskItem, int id)
    {
        throw new NotImplementedException();
    }
}
