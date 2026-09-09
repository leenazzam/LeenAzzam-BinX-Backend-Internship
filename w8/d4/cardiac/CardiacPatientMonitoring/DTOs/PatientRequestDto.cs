using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.DTOs;

public class PatientRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
}