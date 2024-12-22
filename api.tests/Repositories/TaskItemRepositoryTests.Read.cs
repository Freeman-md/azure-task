using System;
using api.Models;
using api.Tests.Builders;

namespace api.tests.Repositories;

public partial class TaskItemRepositoryTests
{
    [Fact]
    public async Task GetOne_ShouldReturnTaskItem_WhenIdIsValid() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().Build();

            TaskItem createdTaskItem = await _repository.Create(taskItem);
        #endregion

        #region Act
            TaskItem? retrievedTaskItem = await _repository.GetOne(createdTaskItem.Id);
        #endregion

        #region Assert
            Assert.NotNull(retrievedTaskItem);
            Assert.Equal(createdTaskItem.Id, retrievedTaskItem.Id);
            Assert.Equal(createdTaskItem.Title, retrievedTaskItem.Title);
        #endregion
    }

    [Fact]
    public async Task GetOne_ShouldReturnNull_WhenIdIsInvalid() {
        #region Act
            TaskItem? retrievedTaskItem = await _repository.GetOne(-1);
        #endregion

        #region Assert
            Assert.Null(retrievedTaskItem);
        #endregion
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllTaskItems() {
        #region Arrange
            IReadOnlyList<TaskItem> taskItems = TaskItemBuilder.BuildMany(3);

            foreach (TaskItem taskItem in taskItems) {
                await _repository.Create(taskItem);
            }
        #endregion

        #region Assert
            IReadOnlyList<TaskItem> retrievedTaskItems = await _repository.GetAll();
        #endregion

        #region Assert
        Assert.NotNull(retrievedTaskItems);
        Assert.Equal(taskItems.Count, retrievedTaskItems.Count);
        #endregion
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoTaskItemsExist() {
        #region Act
            IReadOnlyList<TaskItem> taskItems = await _repository.GetAll();
        #endregion

        #region Name
            Assert.NotNull(taskItems);
            Assert.Empty(taskItems);
        #endregion
    }
}
