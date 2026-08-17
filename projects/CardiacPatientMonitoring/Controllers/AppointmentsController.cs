using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace CardiacPatientMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

[HttpGet]
[Authorize(Roles = "Admin,Doctor,Patient")]    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointments()
    {
        var appointments = await _context.Appointments
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                DoctorName = a.DoctorName,
                Notes = a.Notes
            })
            .ToListAsync();

        return Ok(appointments);
    }

[HttpGet("{id}")]
[Authorize(Roles = "Admin,Doctor,Patient")]    public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
        {
            return NotFound();
        }

        var result = new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            AppointmentDate = appointment.AppointmentDate,
            DoctorName = appointment.DoctorName,
            Notes = appointment.Notes
        };

        return Ok(result);
    }

[HttpPost]
[Authorize(Roles = "Admin,Doctor")]    public async Task<ActionResult<AppointmentResponseDto>> AddAppointment(AppointmentRequestDto request)
    {
        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            AppointmentDate = request.AppointmentDate,
            DoctorName = request.DoctorName,
            Notes = request.Notes
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var result = new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            AppointmentDate = appointment.AppointmentDate,
            DoctorName = appointment.DoctorName,
            Notes = appointment.Notes
        };

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, result);
    }

[HttpPut("{id}")]
[Authorize(Roles = "Admin,Doctor")]    public async Task<IActionResult> UpdateAppointment(int id, AppointmentRequestDto request)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
        {
            return NotFound();
        }

        appointment.PatientId = request.PatientId;
        appointment.AppointmentDate = request.AppointmentDate;
        appointment.DoctorName = request.DoctorName;
        appointment.Notes = request.Notes;

        await _context.SaveChangesAsync();

        return NoContent();
    }

[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
        {
            return NotFound();
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
