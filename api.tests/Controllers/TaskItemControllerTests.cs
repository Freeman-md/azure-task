using System;
using api.Contracts;
using api.Controllers;
using api.Models;
using api.Tests.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace api.tests.Controllers;

public class TaskItemControllerTests
{
    private readonly Mock<ITaskItemRepository> _repository;
    private readonly Mock<ILogger<TaskItemController>> _logger;
    private readonly TaskItemController _controller;

    public TaskItemControllerTests()
    {
        _repository = new Mock<ITaskItemRepository>();
        _logger = new Mock<ILogger<TaskItemController>>();

        _controller = new TaskItemController(_repository.Object, _logger.Object);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WithTaskItems() {
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

    [Fact]
    public async Task Show_ReturnsOkResult_WithValidId() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().WithId(1).Build();
            
            _repository.Setup(r => r.GetOne(taskItem.Id)).ReturnsAsync(taskItem);
        #endregion

        #region Act
            ActionResult<TaskItem> result = await _controller.Show(taskItem.Id);
        #endregion

        #region Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            TaskItem retrievedTaskItem = Assert.IsAssignableFrom<TaskItem>(okResult.Value);

            Assert.Equal(taskItem.Id, retrievedTaskItem.Id);
            Assert.Equal(taskItem.Title, retrievedTaskItem.Title);
        #endregion
    }

    [Fact]
    public async Task Show_ReturnsNotFoundResult_WithInvalidId() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().WithId(1).Build();
            
            _repository.Setup(r => r.GetOne(taskItem.Id)).ReturnsAsync((TaskItem)null!);
        #endregion

        #region Act
            ActionResult<TaskItem> result = await _controller.Show(taskItem.Id);
        #endregion

        #region Assert
            NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        #endregion
    }

    [Fact]
    public async Task Store_SavesTaskItem_ReturnsCreatedTaskItem() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().Build();
            
            _repository.Setup(r => r.Create(taskItem)).ReturnsAsync(taskItem);
        #endregion

        #region Name
            ActionResult<TaskItem> result = await _controller.Create(taskItem);
        #endregion

        #region Assert
            CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            TaskItem createdTaskItem = Assert.IsAssignableFrom<TaskItem>(createdResult.Value);

            Assert.Equal(taskItem.Id, createdTaskItem.Id);
            Assert.Equal(taskItem.Title, createdTaskItem.Title);
        #endregion
    }

    [Fact]
    public async Task Store_ReturnsBadRequest_WithInvalidTaskItem() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().WithId(0).Build();
        #endregion

        #region Act
            ActionResult<TaskItem> result = await _controller.Create(taskItem);
        #endregion

        #region Assert
            BadRequestResult badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
        #endregion
    }
}
