using System;
using api.Models;
using api.Tests.Builders;

namespace api.tests.Repositories;

public partial class TaskItemRepositoryTests
{
    [Fact]
    public async Task Update_ShouldUpdateTaskItem_WhenIdIsValid()
    {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().Build();

            TaskItem createdTaskItem = await _repository.Create(taskItem);
        #endregion

        #region Act
            TaskItem updatedTaskItem = new TaskItemBuilder()
                                            .WithTitle(Guid.NewGuid().ToString())
                                            .WithStatus(TaskItemStatus.InProgress)
                                            .Build();

            TaskItem retrievedTaskItem = await _repository.Update(updatedTaskItem, createdTaskItem.Id);
        #endregion

        #region Assert
            Assert.NotNull(retrievedTaskItem);
            Assert.Equal(createdTaskItem.Id, retrievedTaskItem.Id);
            Assert.Equal(updatedTaskItem.Title, retrievedTaskItem.Title);
            Assert.Equal(updatedTaskItem.Status, retrievedTaskItem.Status);
        #endregion
    }

    [Fact]
    public async Task Update_ShouldThrowException_WhenIdIsInvalid()
    {
        #region Act & Assert
            TaskItem taskItem = new TaskItemBuilder().Build();

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.Update(taskItem, -1));
        #endregion
    }
}
