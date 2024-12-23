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
        public async Task<ActionResult<IReadOnlyList<TaskItemDTO>>> Index()
        {
            IReadOnlyList<TaskItem> taskItems = await _repository.GetAll();

            var taskItemDTOs = taskItems.Select(TaskItemDTO.FromEntity).ToList();

            _logger.LogInformation("Retrieved all task items.");

            return Ok(taskItemDTOs);
        }

        /// <summary>
        /// Retrieves a specific task item by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDTO>> Show(int id)
        {
            TaskItem? taskItem = await _repository.GetOne(id);
            if (taskItem == null)
            {
                _logger.LogWarning("Task item with ID {Id} not found.", id);
                return NotFound();
            }

            var taskItemDTO = TaskItemDTO.FromEntity(taskItem);

            _logger.LogInformation("Retrieved task item with ID {Id}.", id);
            return Ok(taskItemDTO);
        }

        [HttpPost("")]
        public async Task<ActionResult<TaskItemDTO>> Create([FromBody] TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid task item model.");
                return BadRequest(ModelState);
            }

            TaskItem createdTaskItem = await _repository.Create(taskItem);

            _logger.LogInformation("Created task item with ID {Id}.", createdTaskItem.Id);
            return CreatedAtAction(nameof(Show), new { id = createdTaskItem.Id }, TaskItemDTO.FromEntity(createdTaskItem));
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<TaskItemDTO>> Update(int id, [FromBody] TaskItemUpdateDTO taskItemUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid update model for task item with ID {Id}.", id);
                    return BadRequest(ModelState);
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
                    _logger.LogWarning("No valid fields provided for update.");
                    return BadRequest("No fields to update were provided.");
                }

                TaskItem updatedTaskItem = await _repository.Update(id, updatesDict);

                _logger.LogInformation("Updated task item with ID {Id}.", id);
                return Ok(TaskItemDTO.FromEntity(updatedTaskItem));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Destroy(int id)
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
                return NotFound(ex.Message);
            }
        }
    }
}
