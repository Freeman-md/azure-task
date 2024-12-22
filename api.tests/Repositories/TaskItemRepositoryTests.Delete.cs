using System;
using api.Models;
using api.Tests.Builders;

namespace api.tests.Repositories;

public partial class TaskItemRepositoryTests
{
    [Fact]
    public async Task Delete_ShouldRemoveTaskItem_WhenIdIsValid() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().Build();

            TaskItem createdTaskItem = await _repository.Create(taskItem);
        #endregion

        #region Act
            await _repository.Delete(createdTaskItem.Id);

            TaskItem? nullTaskItem = await _repository.GetOne(createdTaskItem.Id);
        #endregion        

        #region Assert
            Assert.Null(nullTaskItem);
        #endregion
    }

    [Fact]
    public async Task Delete_ShouldThrowException_WhenIdIsInvalid() {
        #region Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.Delete(-1));
        #endregion
    }
}
