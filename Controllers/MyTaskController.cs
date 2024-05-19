using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTask.Interfaces;
using MyTask.Models;

namespace MyTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "User")]
    public class MyTaskController : ControllerBase
    {
        private int userId;
        IMyTaskService TaskService;

        public MyTaskController(
            IMyTaskService TaskService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.TaskService = TaskService;
            string id = httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            this.userId = int.Parse(id != null ? id : "0");
        }

        [HttpGet]
        public ActionResult<List<TheTask>> GetAll()
        {
            return TaskService.GetAll(this.userId);
        }

        [HttpGet("{id}")]
        public ActionResult<TheTask> Get(int id)
        {
            var task = TaskService.Get(this.userId, id);

            if (task == null)
                return NotFound();
            return task;
        }

        [HttpPost]
        public IActionResult Create( TheTask task)
        {
            TaskService.Add(this.userId, task);
            return CreatedAtAction(nameof(Create), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int userId, int id, TheTask task)
        {
            if (id != task.Id)
                return BadRequest();

            var existingTask = TaskService.Get(userId, id);
            if (existingTask is null)
                return NotFound();

            TaskService.Update(userId, task);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int userId, int id)
        {
            var task = TaskService.Get(userId, id);
            if (task is null)
                return NotFound();

            TaskService.Delete(userId, id);

            return Content(TaskService.Count.ToString());
        }
    }
}
