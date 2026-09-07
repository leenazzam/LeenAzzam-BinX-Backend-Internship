using FluentValidation;
using CardiacPatientMonitoring.DTOs;

namespace CardiacPatientMonitoring.Validators;

public class VitalSignRequestDtoValidator : AbstractValidator<VitalSignRequestDto>
{
    public VitalSignRequestDtoValidator()
    {
        RuleFor(v => v.PatientId)
            .GreaterThan(0).WithMessage("PatientId must be greater than 0.");

        RuleFor(v => v.HeartRate)
            .InclusiveBetween(30, 220).WithMessage("Heart rate must be between 30 and 220.");

        RuleFor(v => v.BloodPressure)
            .NotEmpty().WithMessage("Blood pressure is required.")
            .Matches(@"^\d{2,3}/\d{2,3}$").WithMessage("Blood pressure must be in format like 120/80.");

        RuleFor(v => v.OxygenLevel)
            .InclusiveBetween(50, 100).WithMessage("Oxygen level must be between 50 and 100.");

        RuleFor(v => v.RecordedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Recorded date cannot be in the future.");
    }
}