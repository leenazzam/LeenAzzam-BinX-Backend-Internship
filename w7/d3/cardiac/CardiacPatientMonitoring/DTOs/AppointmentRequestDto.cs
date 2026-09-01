using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.DTOs;

public class AppointmentRequestDto
{
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(100)]
    public string DoctorName { get; set; } = string.Empty;

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
}