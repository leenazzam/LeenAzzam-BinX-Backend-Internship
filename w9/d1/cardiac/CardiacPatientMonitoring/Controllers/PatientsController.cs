using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;

    public PatientsController(
        AppDbContext context,
        IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: api/patients
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetPatients()
    {
        var patients = await _context.Patients
            .Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Age = p.Age,
                Gender = p.Gender,
                PhoneNumber = p.PhoneNumber
            })
            .ToListAsync();

        return Ok(patients);
    }

    // GET: api/patients/{id}
    // A Patient can view their own profile;
    // Admin/Doctor can view any profile.
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<PatientResponseDto>> GetPatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        if (User.IsInRole("Patient") &&
            !User.IsInRole("Admin") &&
            !User.IsInRole("Doctor"))
        {
            var ownPatientId = User.FindFirstValue("PatientId");

            if (!int.TryParse(ownPatientId, out var parsedPatientId) ||
                patient.Id != parsedPatientId)
            {
                return NotFound();
            }
        }

        var result = new PatientResponseDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            Age = patient.Age,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber
        };

        return Ok(result);
    }

    // POST: api/patients
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PatientResponseDto>> AddPatient(
        PatientRequestDto request)
    {
        var patient = new Patient
        {
            FullName = request.FullName,
            Age = request.Age,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber
        };

        _context.Patients.Add(patient);

        await _context.SaveChangesAsync();

        // Invalidate patients summary cache
        await _cache.RemoveAsync("patients:summary");

        var result = new PatientResponseDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            Age = patient.Age,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber
        };

        return CreatedAtAction(
            nameof(GetPatient),
            new { id = patient.Id },
            result);
    }

    // PUT: api/patients/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePatient(
        int id,
        PatientRequestDto request)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        patient.FullName = request.FullName;
        patient.Age = request.Age;
        patient.Gender = request.Gender;
        patient.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();

        // Invalidate patients summary cache
        await _cache.RemoveAsync("patients:summary");

        return NoContent();
    }

    // DELETE: api/patients/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);

        await _context.SaveChangesAsync();

        // Invalidate patients summary cache
        await _cache.RemoveAsync("patients:summary");

        return NoContent();
    }

    // GET: api/patients/admin-test
    [HttpGet("admin-test")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminTest()
    {
        return Ok(new
        {
            message = "You are an Admin and can access this endpoint."
        });
    }

    // GET: api/patients/summary
    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<List<PatientSummaryDto>>>
        GetPatientsSummary()
    {
        var cacheKey = "patients:summary";

        // 1. Check Redis first
        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached != null)
        {
            var cachedSummaries =
                JsonSerializer.Deserialize<List<PatientSummaryDto>>(cached);

            if (cachedSummaries != null)
            {
                return Ok(cachedSummaries);
            }
        }

        // 2. Cache miss: query the database
        var summaries = await _context.Patients
            .Select(p => new PatientSummaryDto
            {
                Id = p.Id,
                FullName = p.FullName,
                AlertCount = _context.Alerts.Count(
                    a => a.PatientId == p.Id)
            })
            .ToListAsync();

        // 3. Store result in Redis for 10 minutes
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(summaries),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            });

        // 4. Return result
        return Ok(summaries);
    }
}