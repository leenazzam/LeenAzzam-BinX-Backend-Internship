using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.DTOs;

public class VitalSignRequestDto
{
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Range(30, 220)]
    public int HeartRate { get; set; }

    [Required]
    public string BloodPressure { get; set; } = string.Empty;

    [Range(50, 100)]
    public int OxygenLevel { get; set; }

    [Required]
    public DateTime RecordedAt { get; set; }
}