using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Common;         // For ApiResponse<T> and possibly ApiResponseHelper
using api.Contracts;
using api.Controllers;
using api.DTOs;
using api.Models;
using api.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace api.tests.Controllers
{
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
        public async Task Index_ReturnsObjectResult_WithTaskItems()
        {
            #region Arrange
            IReadOnlyList<TaskItem> taskItems = TaskItemBuilder.BuildMany(3);
            _repository.Setup(r => r.GetAll()).ReturnsAsync(taskItems);
            #endregion

            #region Act
            ActionResult<ApiResponse<IReadOnlyList<TaskItemDTO>>> result = await _controller.Index();
            #endregion

            #region Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<IReadOnlyList<TaskItemDTO>>>(objectResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Payload);
            Assert.Equal(taskItems.Count, apiResponse.Payload.Count);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            #endregion
        }

        [Fact]
        public async Task Index_ReturnsObjectResult_WithEmptyList()
        {
            #region Arrange
            var taskItems = new List<TaskItem>();
            _repository.Setup(r => r.GetAll()).ReturnsAsync(taskItems);
            #endregion

            #region Act
            ActionResult<ApiResponse<IReadOnlyList<TaskItemDTO>>> result = await _controller.Index();
            #endregion

            #region Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<IReadOnlyList<TaskItemDTO>>>(objectResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Payload);
            Assert.Empty(apiResponse.Payload);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            #endregion
        }

        [Fact]
        public async Task Show_ReturnsObjectResult_WithValidId()
        {
            #region Arrange
            var taskItem = new TaskItemBuilder().WithId(1).Build();
            _repository.Setup(r => r.GetOne(taskItem.Id)).ReturnsAsync(taskItem);
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Show(taskItem.Id);
            #endregion

            #region Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(objectResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Payload);
            Assert.Equal(taskItem.Id, apiResponse.Payload.Id);
            Assert.Equal(taskItem.Title, apiResponse.Payload.Title);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            #endregion
        }

        [Fact]
        public async Task Show_ReturnsNotFound_WithInvalidId()
        {
            #region Arrange
            _repository.Setup(r => r.GetOne(It.IsAny<int>())).ReturnsAsync((TaskItem)null!);
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Show(999);
            #endregion

            #region Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(notFoundResult.Value);

            Assert.False(apiResponse.Success);
            Assert.Null(apiResponse.Payload);
            Assert.NotNull(apiResponse.Error);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.Error.Code);
            #endregion
        }

        [Fact]
        public async Task Create_SavesTaskItem_ReturnsCreatedTaskItem()
        {
            #region Arrange
            var taskItem = new TaskItemBuilder().WithId(10).WithTitle("New Task").Build();
            _repository.Setup(r => r.Create(taskItem)).ReturnsAsync(taskItem);
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Create(taskItem);
            #endregion

            #region Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(createdResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Payload);
            Assert.Equal(taskItem.Id, apiResponse.Payload.Id);
            Assert.Equal(taskItem.Title, apiResponse.Payload.Title);
            Assert.Equal(StatusCodes.Status201Created, apiResponse.StatusCode);
            #endregion
        }

        [Theory]
        [InlineData(null, "Description is valid", TaskItemStatus.Pending)]
        [InlineData("", "Description is valid", TaskItemStatus.Pending)]
        [InlineData("Valid Title", "Valid Description", (TaskItemStatus)99)]
        [InlineData("Title that exceeds the maximum allowed length of 100 characters which is way too long for a valid task", "Valid Description", TaskItemStatus.Pending)]
        public async Task Create_ReturnsBadRequest_WithInvalidTaskItem(
            string? title,
            string? description,
            TaskItemStatus status)
        {
            #region Arrange
            var taskItem = new TaskItemBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithStatus(status)
                .Build();

            // Simulate model validation errors:
            if (string.IsNullOrEmpty(title) || (title?.Length > 100))
                _controller.ModelState.AddModelError("Title", "The Title field is required and must not exceed 100 characters.");
            if (!Enum.IsDefined(typeof(TaskItemStatus), status))
                _controller.ModelState.AddModelError("Status", "The Status field contains an invalid value.");
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Create(taskItem);
            #endregion

            #region Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(badRequestResult.Value);

            Assert.False(apiResponse.Success);
            Assert.Null(apiResponse.Payload);
            Assert.NotNull(apiResponse.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.True(_controller.ModelState.ErrorCount > 0);
            #endregion
        }

        [Fact]
        public async Task Update_ReturnsObjectResult_WithValidPartialTaskItem()
        {
            #region Arrange
            var taskItem = new TaskItemBuilder().WithId(1).Build();
            var updatedTaskItem = new TaskItemBuilder().WithId(1).WithStatus(TaskItemStatus.Completed).Build();
            var updateDTO = new TaskItemUpdateDTO { Status = TaskItemStatus.Completed };

            _repository.Setup(r => r.Update(taskItem.Id, It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(updatedTaskItem);
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Update(taskItem.Id, updateDTO);
            #endregion

            #region Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(objectResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Payload);
            Assert.Equal(updatedTaskItem.Id, apiResponse.Payload.Id);
            Assert.Equal(TaskItemStatus.Completed.ToString(), apiResponse.Payload.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            #endregion
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WithInvalidPartialTaskItem()
        {
            #region Arrange
            var updateDTO = new TaskItemUpdateDTO
            {
                Title = new string('A', 101),
                Status = (TaskItemStatus)99
            };

            // Simulate model validation errors
            _controller.ModelState.AddModelError("Title", "Title must not exceed 100 characters.");
            _controller.ModelState.AddModelError("Status", "Invalid status value.");
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Update(1, updateDTO);
            #endregion

            #region Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(badRequestResult.Value);

            Assert.False(apiResponse.Success);
            Assert.Null(apiResponse.Payload);
            Assert.NotNull(apiResponse.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.True(_controller.ModelState.ErrorCount > 0);
            #endregion
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WithInvalidId()
        {
            #region Arrange
            var updateDTO = new TaskItemUpdateDTO { Title = "Updated Title" };
            _repository.Setup(repo => repo.Update(It.IsAny<int>(), It.IsAny<Dictionary<string, object>>()))
                .ThrowsAsync(new KeyNotFoundException("Not Found"));
            #endregion

            #region Act
            ActionResult<ApiResponse<TaskItemDTO>> result = await _controller.Update(-1, updateDTO);
            #endregion

            #region Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<TaskItemDTO>>(notFoundResult.Value);

            Assert.False(apiResponse.Success);
            Assert.Null(apiResponse.Payload);
            Assert.NotNull(apiResponse.Error);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.Error.Code);
            #endregion
        }

        [Fact]
        public async Task Destroy_ReturnsNoContent_WithValidId()
        {
            #region Arrange
            // No exception thrown => success => 204 No Content
            _repository.Setup(r => r.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);
            #endregion

            #region Act
            ActionResult<ApiResponse<object>> result = await _controller.Destroy(1);
            #endregion

            #region Assert
            // On success, controller returns NoContent(), so we expect NoContentResult
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
            #endregion
        }

        [Fact]
        public async Task Destroy_ReturnsNotFound_WithInvalidId()
        {
            #region Arrange
            _repository.Setup(r => r.Delete(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Not Found"));
            #endregion

            #region Act
            ActionResult<ApiResponse<object>> result = await _controller.Destroy(-1);
            #endregion

            #region Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);

            Assert.False(apiResponse.Success);
            Assert.Null(apiResponse.Payload);
            Assert.NotNull(apiResponse.Error);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.Error.Code);
            #endregion
        }
    }
}
