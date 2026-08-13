using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.models;
namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]
    [EnableRateLimiting("General")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppTask>>> GetTasks()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .ToListAsync();
        }


        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppTask>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            return task;
        }


        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<AppTask>> CreateTask(
     CreateTaskRequest request)
        {
            var task = new AppTask
            {
                Title = request.Title,
                Status = request.Status,
                DueDate = request.DueDate,
                ProjectId = request.ProjectId
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTask),
                new { id = task.Id },
                task
            );
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
     int id,
     UpdateTaskRequest request)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            task.Title = request.Title;
            task.Status = request.Status;
            task.DueDate = request.DueDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}