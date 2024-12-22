using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;
using api.Tests.Builders;
using Xunit;

namespace api.tests.Repositories
{
    public partial class TaskItemRepositoryTests
    {
        [Fact]
        public async Task Update_ShouldUpdateTaskItem_WhenIdIsValid()
        {
            #region Arrange
            TaskItem taskItem = new TaskItemBuilder().Build();
            TaskItem createdTaskItem = await _repository.Create(taskItem);

            var updates = new Dictionary<string, object>
            {
                { "Title", Guid.NewGuid().ToString() },
                { "Status", TaskItemStatus.InProgress }
            };
            #endregion

            #region Act
            TaskItem retrievedTaskItem = await _repository.Update(createdTaskItem.Id, updates);
            #endregion

            #region Assert
            Assert.NotNull(retrievedTaskItem);
            Assert.Equal(createdTaskItem.Id, retrievedTaskItem.Id);
            Assert.Equal(updates["Title"], retrievedTaskItem.Title);
            Assert.Equal(updates["Status"], retrievedTaskItem.Status);
            #endregion
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenIdIsInvalid()
        {
            #region Arrange
            var updates = new Dictionary<string, object>
            {
                { "Title", Guid.NewGuid().ToString() },
                { "Status", TaskItemStatus.InProgress }
            };
            #endregion

            #region Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.Update(-1, updates));
            #endregion
        }
    }
}