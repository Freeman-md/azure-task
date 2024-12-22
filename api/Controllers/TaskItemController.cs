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
            throw new NotImplementedException();
        }
    }
}
