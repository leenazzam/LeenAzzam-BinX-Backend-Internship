namespace CardiacPatientMonitoring.DTOs;

public class PatientSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int AlertCount { get; set; }
}