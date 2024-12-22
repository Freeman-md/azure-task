using System;
using api.Contracts;
using api.Controllers;
using api.Models;
using api.Tests.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace api.tests.Controllers;

public class TaskItemControllerTests
{
    private readonly Mock<ITaskItemRepository> _repository;
    private readonly TaskItemController _controller;

    public TaskItemControllerTests()
    {
        _repository = new Mock<ITaskItemRepository>();

        _controller = new TaskItemController(_repository.Object);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WithListOfTaskItems() {
        #region Arrange
            IReadOnlyList<TaskItem> taskItems = TaskItemBuilder.BuildMany(3);

            _repository.Setup(r => r.GetAll()).ReturnsAsync(taskItems);
        #endregion

        #region Act
            ActionResult<IReadOnlyList<TaskItem>> result = await _controller.Index();
        #endregion

        #region Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IReadOnlyList<TaskItem> retrievedTaskItems = Assert.IsAssignableFrom<IReadOnlyList<TaskItem>>(okResult.Value);

            Assert.Equal(taskItems.Count, retrievedTaskItems.Count);
        #endregion
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WithEmptyList() {
        #region Arrange
            List<TaskItem> taskItems = new List<TaskItem>();

            _repository.Setup(r => r.GetAll()).ReturnsAsync(taskItems);
        #endregion

        #region Act
            ActionResult<IReadOnlyList<TaskItem>> result = await _controller.Index();
        #endregion

        #region Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IReadOnlyList<TaskItem> retrievedTaskItems = Assert.IsAssignableFrom<IReadOnlyList<TaskItem>>(okResult.Value);

            Assert.Equal(taskItems.Count, retrievedTaskItems.Count);
        #endregion
    }
}
