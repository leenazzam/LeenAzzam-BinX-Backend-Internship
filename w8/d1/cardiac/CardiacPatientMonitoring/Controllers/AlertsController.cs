using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlertsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/alerts?page=1&pageSize=20&patientId=1&unresolvedOnly=true
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<PagedResultDto<AlertResponseDto>>> GetAlerts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? patientId = null,
        [FromQuery] bool? unresolvedOnly = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Alerts.AsQueryable();

        if (patientId.HasValue)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        }

        if (unresolvedOnly == true)
        {
            query = query.Where(a => !a.IsResolved);
        }

        query = query.OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AlertResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                VitalSignId = a.VitalSignId,
                Message = a.Message,
                Severity = a.Severity,
                CreatedAt = a.CreatedAt,
                IsResolved = a.IsResolved
            })
            .ToListAsync();

        return Ok(new PagedResultDto<AlertResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }
}
