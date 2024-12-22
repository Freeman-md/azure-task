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

        public TaskItemController(ITaskItemRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves all task items.
        /// </summary>
        [HttpGet("")]
        public async Task<ActionResult<IReadOnlyList<TaskItem>>> Index()
        {
            IReadOnlyList<TaskItem> taskItems = await _repository.GetAll();

            return Ok(taskItems);
        }
    }
}
