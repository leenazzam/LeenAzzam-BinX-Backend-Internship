using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("General")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public ProjectsController(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }
        // PUT: api/projects/5
[HttpPut("{id}")]
public async Task<IActionResult> UpdateProject(int id, UpdateProjectRequest request)
{
    var project = await _context.Projects.FindAsync(id);

    if (project == null)
        return NotFound();

    if (!User.IsInRole("Admin"))
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (project.OwnerId != ownerId)
            return NotFound();
    }

   project.Name = request.Name;
project.Description = request.Description;

await _context.SaveChangesAsync();

await _cache.RemoveAsync("projects:summary:admin");
await _cache.RemoveAsync($"projects:summary:user:{project.OwnerId}");

return NoContent();
}

// DELETE: api/projects/5
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProject(int id)
{
    var project = await _context.Projects.FindAsync(id);

    if (project == null)
        return NotFound();

    if (!User.IsInRole("Admin"))
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (project.OwnerId != currentUserId)
            return NotFound();
    }

    var ownerId = project.OwnerId;

    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();

    await _cache.RemoveAsync("projects:summary:admin");
    await _cache.RemoveAsync($"projects:summary:user:{ownerId}");

    return NoContent();
}
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectResponseDto>>> GetProjects(
     int page = 1,
     int pageSize = 10,
     string? name = null,
     string? sort = null)
        {
            var query = _context.Projects.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            // Sorting
            query = sort switch
            {
                "name_desc" => query.OrderByDescending(p => p.Name),
                "createdDate" => query.OrderBy(p => p.CreatedDate),
                "createdDate_desc" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderBy(p => p.Name)
            };

                       if (!User.IsInRole("Admin"))
            {
                var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                query = query.Where(p => p.OwnerId == ownerId);
            }

            var totalCount = await query.CountAsync();

            var projects = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProjectResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    OwnerId = p.OwnerId
                })
                .ToListAsync();

            var result = new PagedResult<ProjectResponseDto>
            {
                Items = projects,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return NotFound();
             if (!User.IsInRole("Admin"))
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (project.OwnerId != ownerId)
            return NotFound();
    }

            return project;
        }

       [HttpPost]
public async Task<ActionResult<Project>> CreateProject(CreateProjectRequest request)
{
    var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

    var project = new Project
    {
        Name = request.Name,
        Description = request.Description,
        CreatedDate = DateTime.UtcNow,
        OwnerId = ownerId!
    };
_context.Projects.Add(project);
await _context.SaveChangesAsync();

await _cache.RemoveAsync("projects:summary:admin");
await _cache.RemoveAsync($"projects:summary:user:{ownerId}");

return CreatedAtAction(
    nameof(GetProject),
    new { id = project.Id },
    project
);
}
        [HttpGet("admin")]
        [Authorize(Policy = "AdminWithEmail")]
        public IActionResult AdminOnly()
        {
            return Ok(new
            {
                message = "Welcome Admin!"
            });
        }
// GET: api/projects/summary
[HttpGet("summary")]
public async Task<ActionResult<List<ProjectSummaryDto>>> GetProjectsSummary()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    // Admin sees all projects; regular users see only their own projects.
    var cacheKey = User.IsInRole("Admin")
        ? "projects:summary:admin"
        : $"projects:summary:user:{userId}";

    // 1. Check Redis first
    var cached = await _cache.GetStringAsync(cacheKey);

    if (cached != null)
    {
        var cachedSummaries =
            JsonSerializer.Deserialize<List<ProjectSummaryDto>>(cached);

        if (cachedSummaries != null)
            return Ok(cachedSummaries);
    }

    // 2. Cache miss: query the database
    var query = _context.Projects.AsQueryable();

    if (!User.IsInRole("Admin"))
    {
        query = query.Where(p => p.OwnerId == userId);
    }

    var summaries = await query
        .Select(p => new ProjectSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            TaskCount = _context.Tasks.Count(t => t.ProjectId == p.Id)
        })
        .ToListAsync();

    // 3. Store the result in Redis for 10 minutes
    await _cache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(summaries),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

    // 4. Return the result
    return Ok(summaries);
}

    }
    
}