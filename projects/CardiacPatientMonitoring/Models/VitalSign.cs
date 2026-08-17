namespace CardiacPatientMonitoring.Models;

public class VitalSign
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public int OxygenLevel { get; set; }
    public DateTime RecordedAt { get; set; }
}
