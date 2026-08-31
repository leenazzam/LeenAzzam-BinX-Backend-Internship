using FluentValidation;
using CardiacPatientMonitoring.DTOs;

namespace CardiacPatientMonitoring.Validators;

public class MedicationRequestDtoValidator : AbstractValidator<MedicationRequestDto>
{
    public MedicationRequestDtoValidator()
    {
        RuleFor(m => m.PatientId)
            .GreaterThan(0).WithMessage("PatientId must be greater than 0.");

        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("Medication name is required.")
            .MaximumLength(100);

        RuleFor(m => m.Dosage)
            .NotEmpty().WithMessage("Dosage is required.");

        RuleFor(m => m.Frequency)
            .NotEmpty().WithMessage("Frequency is required.");
    }
}