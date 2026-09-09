using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.DTOs;
using CardiacPatientMonitoring.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VitalSignsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly VitalSignService _vitalSignService;

    public VitalSignsController(AppDbContext context, VitalSignService vitalSignService)
    {
        _context = context;
        _vitalSignService = vitalSignService;
    }

    // GET: api/vitalsigns?page=1&pageSize=20&patientId=1&criticalOnly=true&sortBy=recordedAt&sortDir=desc
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<PagedResultDto<VitalSignResponseDto>>> GetVitalSigns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? patientId = null,
        [FromQuery] int? minHeartRate = null,
        [FromQuery] int? maxHeartRate = null,
        [FromQuery] bool? criticalOnly = null,
        [FromQuery] string sortBy = "recordedAt",
        [FromQuery] string sortDir = "desc")
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.VitalSigns.AsQueryable();

        // Ownership: a Patient can only see their own vital signs
        if (User.IsInRole("Patient") && !User.IsInRole("Admin") && !User.IsInRole("Doctor"))
        {
            var ownPatientId = User.FindFirstValue("PatientId");

            if (!int.TryParse(ownPatientId, out var parsedPatientId))
            {
                return Forbid();
            }

            query = query.Where(v => v.PatientId == parsedPatientId);
        }
        else if (patientId.HasValue)
        {
            query = query.Where(v => v.PatientId == patientId.Value);
        }

        if (minHeartRate.HasValue)
        {
            query = query.Where(v => v.HeartRate >= minHeartRate.Value);
        }

        if (maxHeartRate.HasValue)
        {
            query = query.Where(v => v.HeartRate <= maxHeartRate.Value);
        }

        if (criticalOnly == true)
        {
            query = query.Where(v =>
                v.HeartRate > 150 || v.HeartRate < 40 || v.OxygenLevel < 90);
        }

        query = (sortBy.ToLower(), sortDir.ToLower()) switch
        {
            ("heartrate", "asc") => query.OrderBy(v => v.HeartRate),
            ("heartrate", "desc") => query.OrderByDescending(v => v.HeartRate),
            ("recordedat", "asc") => query.OrderBy(v => v.RecordedAt),
            _ => query.OrderByDescending(v => v.RecordedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VitalSignResponseDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                HeartRate = v.HeartRate,
                BloodPressure = v.BloodPressure,
                OxygenLevel = v.OxygenLevel,
                RecordedAt = v.RecordedAt
            })
            .ToListAsync();

        return Ok(new PagedResultDto<VitalSignResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<VitalSignResponseDto>> GetVitalSign(int id)
    {
        var vitalSign = await _context.VitalSigns.FindAsync(id);

        if (vitalSign == null)
        {
            return NotFound();
        }

        if (User.IsInRole("Patient") && !User.IsInRole("Admin") && !User.IsInRole("Doctor"))
        {
            var ownPatientId = User.FindFirstValue("PatientId");

            if (!int.TryParse(ownPatientId, out var parsedPatientId) ||
                vitalSign.PatientId != parsedPatientId)
            {
                return NotFound();
            }
        }

        var result = new VitalSignResponseDto
        {
            Id = vitalSign.Id,
            PatientId = vitalSign.PatientId,
            HeartRate = vitalSign.HeartRate,
            BloodPressure = vitalSign.BloodPressure,
            OxygenLevel = vitalSign.OxygenLevel,
            RecordedAt = vitalSign.RecordedAt
        };

        return Ok(result);
    }

    // POST: api/vitalsigns
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<VitalSignCreationResultDto>> AddVitalSign(VitalSignRequestDto request)
    {
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == request.PatientId);
        if (!patientExists)
        {
            return BadRequest(new { message = "PatientId does not reference an existing patient." });
        }

        var vitalSign = new VitalSign
        {
            PatientId = request.PatientId,
            HeartRate = request.HeartRate,
            BloodPressure = request.BloodPressure,
            OxygenLevel = request.OxygenLevel,
            RecordedAt = request.RecordedAt
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        Alert? alert = null;

        try
        {
            _context.VitalSigns.Add(vitalSign);
            await _context.SaveChangesAsync();

            var isCritical = _vitalSignService.IsCritical(vitalSign);

            if (isCritical)
            {
                alert = new Alert
                {
                    PatientId = vitalSign.PatientId,
                    VitalSignId = vitalSign.Id,
                    Message = $"Critical reading recorded: HR={vitalSign.HeartRate}, SpO2={vitalSign.OxygenLevel}%.",
                    Severity = "Critical",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        var vitalSignDto = new VitalSignResponseDto
        {
            Id = vitalSign.Id,
            PatientId = vitalSign.PatientId,
            HeartRate = vitalSign.HeartRate,
            BloodPressure = vitalSign.BloodPressure,
            OxygenLevel = vitalSign.OxygenLevel,
            RecordedAt = vitalSign.RecordedAt
        };

        var result = new VitalSignCreationResultDto
        {
            VitalSign = vitalSignDto,
            IsCritical = alert != null,
            Alert = alert == null ? null : new AlertResponseDto
            {
                Id = alert.Id,
                PatientId = alert.PatientId,
                VitalSignId = alert.VitalSignId,
                Message = alert.Message,
                Severity = alert.Severity,
                CreatedAt = alert.CreatedAt,
                IsResolved = alert.IsResolved
            }
        };

        return CreatedAtAction(nameof(GetVitalSign), new { id = vitalSign.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateVitalSign(int id, VitalSignRequestDto request)
    {
        var vitalSign = await _context.VitalSigns.FindAsync(id);

        if (vitalSign == null)
        {
            return NotFound();
        }

        vitalSign.PatientId = request.PatientId;
        vitalSign.HeartRate = request.HeartRate;
        vitalSign.BloodPressure = request.BloodPressure;
        vitalSign.OxygenLevel = request.OxygenLevel;
        vitalSign.RecordedAt = request.RecordedAt;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVitalSign(int id)
    {
        var vitalSign = await _context.VitalSigns.FindAsync(id);

        if (vitalSign == null)
        {
            return NotFound();
        }

        _context.VitalSigns.Remove(vitalSign);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}