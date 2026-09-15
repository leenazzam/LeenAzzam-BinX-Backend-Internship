namespace CardiacPatientMonitoring.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; }
    public string Notes { get; set; }
}
