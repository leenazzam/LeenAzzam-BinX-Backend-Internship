using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicationsController(AppDbContext context)
    {
        _context = context;
    }
/// <summary>
/// Gets medications accessible to the authenticated user.
/// </summary>
/// <response code="200">Medications returned successfully.</response>
[HttpGet]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<MedicationResponseDto>>> GetMedications()
    {
        var query = _context.Medications.AsQueryable();

        if (User.IsInRole("Patient") && !User.IsInRole("Admin") && !User.IsInRole("Doctor"))
        {
            var ownPatientId = User.FindFirstValue("PatientId");

            if (!int.TryParse(ownPatientId, out var parsedPatientId))
            {
                return Forbid();
            }

            query = query.Where(m => m.PatientId == parsedPatientId);
        }

        var medications = await query
            .Select(m => new MedicationResponseDto
            {
                Id = m.Id,
                PatientId = m.PatientId,
                Name = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency
            })
            .ToListAsync();

        return Ok(medications);
    }
/// <summary>
/// Gets a medication by ID.
/// </summary>
/// <param name="id">Medication ID.</param>
/// <response code="200">Medication found.</response>
/// <response code="404">Medication was not found or is not accessible.</response>
[HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<MedicationResponseDto>> GetMedication(int id)
    {
        var medication = await _context.Medications.FindAsync(id);

        if (medication == null)
        {
            return NotFound();
        }

        if (User.IsInRole("Patient") && !User.IsInRole("Admin") && !User.IsInRole("Doctor"))
        {
            var ownPatientId = User.FindFirstValue("PatientId");

            if (!int.TryParse(ownPatientId, out var parsedPatientId) ||
                medication.PatientId != parsedPatientId)
            {
                return NotFound();
            }
        }

        var result = new MedicationResponseDto
        {
            Id = medication.Id,
            PatientId = medication.PatientId,
            Name = medication.Name,
            Dosage = medication.Dosage,
            Frequency = medication.Frequency
        };

        return Ok(result);
    }
/// <summary>
/// Adds a medication to a patient.
/// </summary>
/// <param name="request">Medication information.</param>
/// <response code="201">Medication created successfully.</response>
/// <response code="400">Invalid medication data.</response>
[HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<MedicationResponseDto>> AddMedication(MedicationRequestDto request)
    {
        var medication = new Medication
        {
            PatientId = request.PatientId,
            Name = request.Name,
            Dosage = request.Dosage,
            Frequency = request.Frequency
        };

        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        var result = new MedicationResponseDto
        {
            Id = medication.Id,
            PatientId = medication.PatientId,
            Name = medication.Name,
            Dosage = medication.Dosage,
            Frequency = medication.Frequency
        };

        return CreatedAtAction(nameof(GetMedication), new { id = medication.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateMedication(int id, MedicationRequestDto request)
    {
        var medication = await _context.Medications.FindAsync(id);

        if (medication == null)
        {
            return NotFound();
        }

        medication.PatientId = request.PatientId;
        medication.Name = request.Name;
        medication.Dosage = request.Dosage;
        medication.Frequency = request.Frequency;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMedication(int id)
    {
        var medication = await _context.Medications.FindAsync(id);

        if (medication == null)
        {
            return NotFound();
        }

        _context.Medications.Remove(medication);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}