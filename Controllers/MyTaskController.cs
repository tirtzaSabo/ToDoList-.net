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
        IMyTaskService TaskService;

        public MyTaskController(IMyTaskService TaskService)
        {
            this.TaskService = TaskService;
        }

        [HttpGet]
        public ActionResult<List<TheTask>> GetAll() => TaskService.GetAll();

        [HttpGet("{id}")]
        public ActionResult<TheTask> Get(int id)
        {
            var task = TaskService.Get(id);

            if (task == null)
                return NotFound();

            return task;
        }

        [HttpPost]
        public IActionResult Create(TheTask task)
        {
            TaskService.Add(task);
            return CreatedAtAction(nameof(Create), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, TheTask task)
        {
            if (id != task.Id)
                return BadRequest();

            var existingTask = TaskService.Get(id);
            if (existingTask is null)
                return NotFound();

            TaskService.Update(task);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var task = TaskService.Get(id);
            if (task is null)
                return NotFound();

            TaskService.Delete(id);

            return Content(TaskService.Count.ToString());
        }
    }
}
