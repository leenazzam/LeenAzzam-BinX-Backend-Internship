using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Models;

namespace CardiacPatientMonitoring.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<VitalSign> VitalSigns { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Patient <-> Identity user (one-to-one by shared key)
        builder.Entity<Patient>()
            .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
            .WithMany()
            .HasForeignKey(p => p.IdentityUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Patient -> VitalSigns (one-to-many)
        // A patient's vitals are meaningless without the patient record,
        // so they are removed together with the patient.
        builder.Entity<VitalSign>()
            .HasOne(v => v.Patient)
            .WithMany()
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Patient -> Medications (one-to-many)
        builder.Entity<Medication>()
            .HasOne(m => m.Patient)
            .WithMany()
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Patient -> Appointments (one-to-many)
        builder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Patient -> Alerts (one-to-many)
        // NOTE: not Cascade here. Alert already reaches Patient through VitalSign
        // (Patient -> VitalSign -> Alert, both Cascade), and SQL Server refuses to
        // create two independent cascade paths to the same table. Deleting a patient
        // still deletes their alerts — it just happens via the VitalSign cascade.
        builder.Entity<Alert>()
            .HasOne(al => al.Patient)
            .WithMany()
            .HasForeignKey(al => al.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // VitalSign -> Alerts (one-to-many): the specific reading that triggered the alert.
        builder.Entity<Alert>()
            .HasOne(al => al.VitalSign)
            .WithMany()
            .HasForeignKey(al => al.VitalSignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Alert>()
            .Property(al => al.Severity)
            .HasMaxLength(20);

        builder.Entity<Alert>()
            .Property(al => al.Message)
            .HasMaxLength(300);
    }
}
