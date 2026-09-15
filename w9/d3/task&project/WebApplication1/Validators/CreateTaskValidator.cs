using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
    {
        public CreateTaskValidator()
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

            RuleFor(x => x.ProjectId)
                .GreaterThan(0)
                .WithMessage("ProjectId must be greater than 0.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("DueDate must be in the future.");
        }
    }
}