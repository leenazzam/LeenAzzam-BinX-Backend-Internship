namespace CardiacPatientMonitoring.DTOs;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; }
    public string Notes { get; set; }
}
