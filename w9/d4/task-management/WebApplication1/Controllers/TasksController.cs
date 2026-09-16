using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

/// <summary>
/// Gets a paginated list of tasks.
/// </summary>
/// <param name="page">Page number.</param>
/// <param name="pageSize">Number of tasks per page.</param>
/// <param name="status">Optional task status filter.</param>
/// <param name="projectId">Optional project filter.</param>
/// <param name="sort">Sort order.</param>
/// <response code="200">Returns the requested tasks.</response>
/// <response code="401">User is not authenticated.</response>
[HttpGet]
public async Task<ActionResult<PagedResult<TaskResponseDto>>> GetTasks(
    int page = 1,
    int pageSize = 10,
    string? status = null,
    int? projectId = null,
    string? sort = null)
{
    var query = _context.Tasks.AsQueryable();

    if (!User.IsInRole("Admin"))
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        query = query.Where(t => t.Project != null && t.Project.OwnerId == ownerId);
    }

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
/// <summary>
/// Gets a task by its ID.
/// </summary>
/// <param name="id">The task ID.</param>
/// <response code="200">Task found successfully.</response>
/// <response code="404">Task was not found.</response>
/// <response code="401">User is not authenticated.</response>
[HttpGet("{id}")]
public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
{
    var task = await _context.Tasks
        .Include(t => t.Project)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (task == null)
        return NotFound();

    if (!User.IsInRole("Admin"))
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (task.Project == null || task.Project.OwnerId != ownerId)
            return NotFound();
    }

    return new TaskResponseDto
    {
        Id = task.Id,
        Title = task.Title,
        Status = task.Status,
        DueDate = task.DueDate,
        ProjectId = task.ProjectId,
        ProjectName = task.Project?.Name
    };
}

/// <summary>
/// Creates a new task.
/// </summary>
/// <remarks>
/// Example request:
///
///     POST /api/tasks
///     {
///       "title": "Complete API documentation",
///       "status": "Pending",
///       "dueDate": "2026-09-20T12:00:00Z",
///       "projectId": 1
///     }
///
/// Example response:
///
///     {
///       "id": 25,
///       "title": "Complete API documentation",
///       "status": "Pending",
///       "dueDate": "2026-09-20T12:00:00Z",
///       "projectId": 1
///     }
/// </remarks>
/// <response code="201">Task created successfully.</response>
/// <response code="400">Invalid task data.</response>
[HttpPost]
public async Task<ActionResult<AppTask>> CreateTask(
     CreateTaskRequest request)
{
    var project = await _context.Projects
        .FirstOrDefaultAsync(p => p.Id == request.ProjectId);

    if (project == null)
        return NotFound($"Project with id {request.ProjectId} was not found.");

    if (!User.IsInRole("Admin"))
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (project.OwnerId != ownerId)
            return NotFound($"Project with id {request.ProjectId} was not found.");
    }

    

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

      /// <summary>
/// Updates an existing task.
/// </summary>
/// <param name="id">The task ID.</param>
/// <param name="request">Updated task information.</param>
/// <response code="204">Task updated successfully.</response>
/// <response code="404">Task was not found.</response>
[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(
     int id,
     UpdateTaskRequest request)
{
    var task = await _context.Tasks
        .Include(t => t.Project)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (task == null)
        return NotFound();

    if (!User.IsInRole("Admin"))
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (task.Project == null || task.Project.OwnerId != ownerId)
            return NotFound();
    }

    task.Title = request.Title;
    task.Status = request.Status;
    task.DueDate = request.DueDate;
    task.ProjectId = request.ProjectId;

    await _context.SaveChangesAsync();

    return NoContent();
}
       /// <summary>
/// Deletes a task.
/// </summary>
/// <param name="id">The task ID.</param>
/// <response code="204">Task deleted successfully.</response>
/// <response code="404">Task was not found.</response>
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