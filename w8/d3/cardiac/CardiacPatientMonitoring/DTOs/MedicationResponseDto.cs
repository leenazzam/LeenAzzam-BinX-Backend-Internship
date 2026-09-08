namespace CardiacPatientMonitoring.DTOs;

public class MedicationResponseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Name { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
}
