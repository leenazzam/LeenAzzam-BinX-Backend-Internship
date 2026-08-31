using FluentValidation;
using CardiacPatientMonitoring.DTOs;

namespace CardiacPatientMonitoring.Validators;

public class PatientRequestDtoValidator : AbstractValidator<PatientRequestDto>
{
    public PatientRequestDtoValidator()
    {
        RuleFor(p => p.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.");

        RuleFor(p => p.Age)
            .InclusiveBetween(1, 120).WithMessage("Age must be between 1 and 120.");

        RuleFor(p => p.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => g == "Male" || g == "Female")
            .WithMessage("Gender must be either Male or Female.");

        RuleFor(p => p.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^05\d{8}$").WithMessage("Phone number must be a valid local number starting with 05.");
    }
}