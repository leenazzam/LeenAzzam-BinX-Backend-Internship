namespace CardiacPatientMonitoring.DTOs;

public class VitalSignResponseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public int OxygenLevel { get; set; }
    public DateTime RecordedAt { get; set; }
}
