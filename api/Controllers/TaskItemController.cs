using api.Common;
using api.Contracts;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/task-items")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemRepository _repository;
        private readonly ILogger<TaskItemController> _logger;

        public TaskItemController(ITaskItemRepository repository, ILogger<TaskItemController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all task items.
        /// </summary>
        [HttpGet("")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TaskItemDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<TaskItemDTO>>>> Index()
        {
            IReadOnlyList<TaskItem> taskItems = await _repository.GetAll();
            var taskItemDTOs = taskItems.Select(TaskItemDTO.FromEntity).ToList();

            _logger.LogInformation("Retrieved all task items.");

            return ApiResponseHelper.Success((IReadOnlyList<TaskItemDTO>)taskItemDTOs);
        }

        /// <summary>
        /// Retrieves a specific task item by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TaskItemDTO>>> Show(int id)
        {
            TaskItem? taskItem = await _repository.GetOne(id);
            if (taskItem == null)
            {
                _logger.LogWarning("Task item with ID {Id} not found.", id);
                return ApiResponseHelper.Error<TaskItemDTO>(
                    code: StatusCodes.Status404NotFound,
                    message: $"TaskItem with ID {id} not found.",
                    statusCode: StatusCodes.Status404NotFound
                );
            }

            var taskItemDTO = TaskItemDTO.FromEntity(taskItem);

            _logger.LogInformation("Retrieved task item with ID {Id}.", id);
            return ApiResponseHelper.Success(taskItemDTO);
        }

        /// <summary>
        /// Creates a new task item.
        /// </summary>
        [HttpPost("")]
        [ProducesResponseType(typeof(ApiResponse<TaskItemDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<TaskItemDTO>>> Create([FromBody] TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid task item model.");
                return ApiResponseHelper.Error<TaskItemDTO>(
                    code: StatusCodes.Status400BadRequest,
                    message: "Invalid model state.",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            TaskItem createdTaskItem = await _repository.Create(taskItem);

            _logger.LogInformation("Created task item with ID {Id}.", createdTaskItem.Id);

            var result = TaskItemDTO.FromEntity(createdTaskItem);
            return CreatedAtAction(
                nameof(Show),
                new { id = createdTaskItem.Id },
                new ApiResponse<TaskItemDTO>(true, result, null, StatusCodes.Status201Created)
            );
        }

        /// <summary>
        /// Partially updates a task item.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TaskItemDTO>>> Update(int id, [FromBody] TaskItemUpdateDTO taskItemUpdate)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid update model for task item with ID {Id}.", id);
                return ApiResponseHelper.Error<TaskItemDTO>(
                    code: StatusCodes.Status400BadRequest,
                    message: "Invalid model state.",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            var updatesDict = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(taskItemUpdate.Title))
                updatesDict.Add(nameof(TaskItem.Title), taskItemUpdate.Title);

            if (!string.IsNullOrWhiteSpace(taskItemUpdate.Description))
                updatesDict.Add(nameof(TaskItem.Description), taskItemUpdate.Description);

            if (taskItemUpdate.Status.HasValue)
                updatesDict.Add(nameof(TaskItem.Status), taskItemUpdate.Status.Value);

            if (taskItemUpdate.DueDate.HasValue)
                updatesDict.Add(nameof(TaskItem.DueDate), taskItemUpdate.DueDate.Value);

            if (updatesDict.Count == 0)
            {
                _logger.LogWarning("No valid fields provided for update for TaskItem ID {Id}.", id);
                return ApiResponseHelper.Error<TaskItemDTO>(
                    code: StatusCodes.Status400BadRequest,
                    message: "No fields to update were provided.",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            try
            {
                TaskItem updatedTaskItem = await _repository.Update(id, updatesDict);

                _logger.LogInformation("Updated task item with ID {Id}.", id);
                return ApiResponseHelper.Success(TaskItemDTO.FromEntity(updatedTaskItem));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return ApiResponseHelper.Error<TaskItemDTO>(
                    code: StatusCodes.Status404NotFound,
                    message: ex.Message,
                    statusCode: StatusCodes.Status404NotFound
                );
            }
        }

        /// <summary>
        /// Deletes a task item by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Destroy(int id)
        {
            try
            {
                await _repository.Delete(id);

                _logger.LogInformation("Deleted task item with ID {Id}.", id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return ApiResponseHelper.Error<object>(
                    code: StatusCodes.Status404NotFound,
                    message: ex.Message,
                    statusCode: StatusCodes.Status404NotFound
                );
            }
        }
    }
}
