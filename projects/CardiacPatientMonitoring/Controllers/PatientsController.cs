using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class PatientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

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

   [HttpGet("{id}")]
[Authorize(Roles = "Admin,Doctor")]
public async Task<ActionResult<PatientResponseDto>> GetPatient(int id) {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
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
    }[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<PatientResponseDto>> AddPatient(PatientRequestDto request) {
        var patient = new Patient
        {
            FullName = request.FullName,
            Age = request.Age,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var result = new PatientResponseDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            Age = patient.Age,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber
        };

        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, result);
    }[HttpPut("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UpdatePatient(int id, PatientRequestDto request) {
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

        return NoContent();
    }[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeletePatient(int id) {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("admin-test")]
[Authorize(Roles = "Admin")]
public IActionResult AdminTest()
{
    return Ok(new
    {
        message = "You are an Admin and can access this endpoint."
    });
}
}
