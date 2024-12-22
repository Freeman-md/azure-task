using System;
using api.Contracts;
using api.Models;
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
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TaskItem>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem> GetOne(int id)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem> Update(TaskItem taskItem, int id)
    {
        throw new NotImplementedException();
    }
}
