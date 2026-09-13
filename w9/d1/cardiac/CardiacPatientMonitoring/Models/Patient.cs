namespace CardiacPatientMonitoring.Models;

public class Patient
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    // Link Patient to ASP.NET Core Identity user
    public string IdentityUserId { get; set; } = null!;
}