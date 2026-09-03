namespace CardiacPatientMonitoring.DTOs;

public class AlertResponseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int VitalSignId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}
