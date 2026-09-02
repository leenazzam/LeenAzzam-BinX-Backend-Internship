using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("General")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
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
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (project.OwnerId != ownerId)
            return NotFound();
    }

    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();

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

        

    }
}