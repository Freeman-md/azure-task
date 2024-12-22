using System;
using api.Contracts;
using api.Models;
using api.Repositories;
using api.Tests.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;

namespace api.tests.Repositories;

public partial class TaskItemRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITaskItemRepository _repository;

    public TaskItemRepositoryTests()
    {
        _dbContext = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("Test")
                .Options
        );

        _repository = new TaskItemRepository(_dbContext);
        
        // Clear the TaskItems DbSet to ensure a clean state before each test
        _dbContext.TaskItems.RemoveRange(_dbContext.TaskItems);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Create_ShouldReturnTaskItem_WhenTaskItemIsValid()
    {
        #region Arrange
        TaskItem taskItem = new TaskItemBuilder().Build();
        #endregion

        #region Act
        var createdTaskItem = await _repository.Create(taskItem);
        #endregion

        #region Assert
        Assert.NotNull(createdTaskItem);
        Assert.Equal(taskItem.Title, createdTaskItem.Title);
        #endregion
    }


    [Fact]
    public async Task Create_ShouldThrowException_WhenTaskItemIsNull()
    {
        #region Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Create(null!));
        #endregion
    }
}
