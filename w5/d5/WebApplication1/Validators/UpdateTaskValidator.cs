using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(100)
                .WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(status =>
                    status == "Pending" ||
                    status == "In Progress" ||
                    status == "Completed")
                .WithMessage("Status must be Pending, In Progress, or Completed.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("DueDate must be in the future.");
        }
    }
}