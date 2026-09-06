using FluentValidation;
using CardiacPatientMonitoring.DTOs;

namespace CardiacPatientMonitoring.Validators;

public class AppointmentRequestDtoValidator : AbstractValidator<AppointmentRequestDto>
{
    public AppointmentRequestDtoValidator()
    {
        RuleFor(a => a.PatientId)
            .GreaterThan(0).WithMessage("PatientId must be greater than 0.");

        RuleFor(a => a.AppointmentDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

        RuleFor(a => a.DoctorName)
            .NotEmpty().WithMessage("Doctor name is required.")
            .MaximumLength(100);

        RuleFor(a => a.Notes)
            .MaximumLength(500);
    }
}