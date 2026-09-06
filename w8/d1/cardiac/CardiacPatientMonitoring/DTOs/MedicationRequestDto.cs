using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.DTOs;

public class MedicationRequestDto
{
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Frequency { get; set; } = string.Empty;
}