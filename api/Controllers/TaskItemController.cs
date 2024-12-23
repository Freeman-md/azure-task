using api.Contracts;
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
        public async Task<ActionResult<IReadOnlyList<TaskItem>>> Index()
        {
            IReadOnlyList<TaskItem> taskItems = await _repository.GetAll();

            _logger.LogInformation("Retrieved all task items.");

            return Ok(taskItems);
        }

        /// <summary>
        /// Retrieves a specific task item by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> Show(int id)
        {
            TaskItem? taskItem = await _repository.GetOne(id);
            if (taskItem == null)
            {
                _logger.LogWarning("Task item with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved task item with ID {Id}.", id);
            return Ok(taskItem);
        }

        [HttpPost("")]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid task item model.");
                return BadRequest(ModelState);
            }

            TaskItem createdTaskItem = await _repository.Create(taskItem);

            _logger.LogInformation("Created task item with ID {Id}.", createdTaskItem.Id);
            return CreatedAtAction(nameof(Show), new { id = createdTaskItem.Id }, createdTaskItem);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<TaskItem>> Update(int id, [FromBody] TaskItem taskItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid update model for task item with ID {Id}.", id);
                    return BadRequest(ModelState);
                }

                var updatesDict = new Dictionary<string, object>
                {
                    { "Title", taskItem.Title },
                    { "Description", taskItem.Description ?? string.Empty },
                    { "Status", taskItem.Status },
                    { "DueDate", taskItem.DueDate }
                };

                TaskItem updatedTaskItem = await _repository.Update(id, updatesDict);

                _logger.LogInformation("Updated task item with ID {Id}.", id);
                return Ok(updatedTaskItem);
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
