using System;
using api.Contracts;
using api.Controllers;
using api.Models;
using api.Tests.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
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
    public async Task Index_ReturnsOkResult_WithTaskItems()
    {
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
    public async Task Index_ReturnsOkResult_WithEmptyList()
    {
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
    public async Task Show_ReturnsOkResult_WithValidId()
    {
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
    public async Task Show_ReturnsNotFoundResult_WithInvalidId()
    {
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
    public async Task Store_SavesTaskItem_ReturnsCreatedTaskItem()
    {
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

    [Theory]
    [InlineData(null, "Description is valid", TaskItemStatus.Pending)]
    [InlineData("", "Description is valid", TaskItemStatus.Pending)]
    [InlineData("Valid Title", "Valid Description", (TaskItemStatus)99)]
    [InlineData("Title that exceeds the maximum allowed length of 100 characters which is way too long for a valid task", "Valid Description", TaskItemStatus.Pending)]
    public async Task Store_ReturnsBadRequest_WithInvalidTaskItem(string? title, string? description, TaskItemStatus status)
    {
        #region Arrange
        TaskItem taskItem = new TaskItemBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithStatus(status)
            .Build();

        if (title == null || title == "" || title.Length > 100)
            _controller.ModelState.AddModelError("Title", "The Title field is required and must not exceed 100 characters.");
        if (!Enum.IsDefined(typeof(TaskItemStatus), status))
            _controller.ModelState.AddModelError("Status", "The Status field contains an invalid value.");

        _repository.Setup(r => r.Create(It.IsAny<TaskItem>())).Throws(new Exception("This should not be called"));
        #endregion

        #region Act
        ActionResult<TaskItem> result = await _controller.Create(taskItem);
        #endregion

        #region Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequestResult.Value);

        Assert.True(_controller.ModelState.ErrorCount > 0);
        #endregion
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WithValidTaskItem() {
        #region Arrange
        TaskItem taskItem = new TaskItemBuilder().WithId(1).Build();
        TaskItem updatedTaskItem = new TaskItemBuilder().WithTitle("Updated Title").Build();

        var updatedTaskItemDict = new Dictionary<string, object>
        {
            { "Title", updatedTaskItem.Title },
            { "Description", updatedTaskItem.Description },
            { "Status", updatedTaskItem.Status }
        };

        _repository.Setup(r => r.Update(taskItem.Id, updatedTaskItemDict)).ReturnsAsync(updatedTaskItem);
        #endregion

        #region Act
        ActionResult<TaskItem> result = await _controller.Update(taskItem.Id, updatedTaskItemDict);
        #endregion

        #region Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        TaskItem retrievedTaskItem = Assert.IsAssignableFrom<TaskItem>(okResult.Value);

        Assert.Equal(updatedTaskItem.Id, retrievedTaskItem.Id);
        Assert.Equal(updatedTaskItem.Title, retrievedTaskItem.Title);
        #endregion
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WithInvalidId() {
        #region Arrange
            TaskItem taskItem = new TaskItemBuilder().WithId(1).Build();

            var updatedTaskItemDict = new Dictionary<string, object>
            {
                { "Title", "Updated Title" },
                { "Description", "Updated Description" },
                { "Status", TaskItemStatus.Done }
            };

            _repository.Setup(repo => repo.Update(It.IsAny<int>(), updatedTaskItemDict)).ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act
            ActionResult<TaskItem> result = await _controller.Update(-1, updatedTaskItemDict);
        #endregion

        #region Assert
            Assert.IsType<NotFoundResult>(result.Result);
        #endregion
    }

}
