namespace CardiacPatientMonitoring.Models;

public class Alert
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    public int VitalSignId { get; set; }
    public VitalSign VitalSign { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = "Critical";

    public DateTime CreatedAt { get; set; }

    public bool IsResolved { get; set; } = false;
}
