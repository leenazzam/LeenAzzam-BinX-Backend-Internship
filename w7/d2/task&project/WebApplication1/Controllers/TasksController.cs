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
public async Task<ActionResult<PagedResult<TaskResponseDto>>> GetTasks(
    int page = 1,
    int pageSize = 10,
    string? status = null,
    int? projectId = null,
    string? sort = null)
{
    var query = _context.Tasks
        .Include(t => t.Project)
        .AsQueryable();

    // Filtering
    if (!string.IsNullOrEmpty(status))
        query = query.Where(t => t.Status == status);

    if (projectId.HasValue)
        query = query.Where(t => t.ProjectId == projectId.Value);

    // Sorting
    query = sort switch
    {
        "dueDate" => query.OrderBy(t => t.DueDate),
        "dueDate_desc" => query.OrderByDescending(t => t.DueDate),
        "title_desc" => query.OrderByDescending(t => t.Title),
        _ => query.OrderBy(t => t.Title)
    };

    var totalCount = await query.CountAsync();

    var tasks = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            DueDate = t.DueDate,
            ProjectId = t.ProjectId,
            ProjectName = t.Project != null ? t.Project.Name : null
        })
        .ToListAsync();

    var result = new PagedResult<TaskResponseDto>
    {
        Items = tasks,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };

    return Ok(result);
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
    var projectExists = await _context.Projects
        .AnyAsync(p => p.Id == request.ProjectId);

    if (!projectExists)
        return NotFound($"Project with id {request.ProjectId} was not found.");

    var duplicateExists = await _context.Tasks
        .AnyAsync(t => t.ProjectId == request.ProjectId && t.Title == request.Title);

    if (duplicateExists)
        return BadRequest("A task with the same title already exists in this project.");

    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        var task = new AppTask
        {
            Title = request.Title,
            Status = string.IsNullOrEmpty(request.Status) ? "Pending" : request.Status,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return CreatedAtAction(
            nameof(GetTask),
            new { id = task.Id },
            task
        );
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
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
            task.ProjectId = request.ProjectId;

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