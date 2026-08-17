using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VitalSignsController : ControllerBase
{
    private readonly AppDbContext _context;

    public VitalSignsController(AppDbContext context)
    {
        _context = context;
    }
[HttpGet]
[Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> GetVitalSigns()
    {
        var vitalSigns = await _context.VitalSigns
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

        return Ok(vitalSigns);
    }

[HttpGet("{id}")]
[Authorize(Roles = "Admin,Doctor,Patient")]    public async Task<ActionResult<VitalSignResponseDto>> GetVitalSign(int id)
    {
        var vitalSign = await _context.VitalSigns.FindAsync(id);

        if (vitalSign == null)
        {
            return NotFound();
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

[HttpPost]
[Authorize(Roles = "Admin,Doctor")]    public async Task<ActionResult<VitalSignResponseDto>> AddVitalSign(VitalSignRequestDto request)
    {
        var vitalSign = new VitalSign
        {
            PatientId = request.PatientId,
            HeartRate = request.HeartRate,
            BloodPressure = request.BloodPressure,
            OxygenLevel = request.OxygenLevel,
            RecordedAt = request.RecordedAt
        };

        _context.VitalSigns.Add(vitalSign);
        await _context.SaveChangesAsync();

        var result = new VitalSignResponseDto
        {
            Id = vitalSign.Id,
            PatientId = vitalSign.PatientId,
            HeartRate = vitalSign.HeartRate,
            BloodPressure = vitalSign.BloodPressure,
            OxygenLevel = vitalSign.OxygenLevel,
            RecordedAt = vitalSign.RecordedAt
        };

        return CreatedAtAction(nameof(GetVitalSign), new { id = vitalSign.Id }, result);
    }

[HttpPut("{id}")]
[Authorize(Roles = "Admin,Doctor")]    public async Task<IActionResult> UpdateVitalSign(int id, VitalSignRequestDto request)
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
[Authorize(Roles = "Admin")]    public async Task<IActionResult> DeleteVitalSign(int id)
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
