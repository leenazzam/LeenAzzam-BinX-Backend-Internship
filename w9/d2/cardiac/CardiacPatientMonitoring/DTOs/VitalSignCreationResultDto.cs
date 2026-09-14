namespace CardiacPatientMonitoring.DTOs;

public class VitalSignCreationResultDto
{
    public VitalSignResponseDto VitalSign { get; set; } = null!;
    public bool IsCritical { get; set; }
    public AlertResponseDto? Alert { get; set; }
}
