using FluentValidation;

namespace proj_mngmt_api.Features.ProjectsManagement.Tasks
{
  public class TaskItemValidator : AbstractValidator<TaskItemDto>
  {
    public TaskItemValidator()
    {
      RuleLevelCascadeMode = CascadeMode.Stop;

      RuleFor(x => x.Title).NotEmpty().MaximumLength(30);
      RuleFor(x => x.Description).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Description));
      RuleFor(x => x.Assignee).MaximumLength(20);
      RuleFor(x => x.Type).NotNull().IsInEnum();
      RuleFor(x => x.Priority).NotEmpty().IsInEnum();
      RuleFor(x => x.Status).NotEmpty().IsInEnum();
      RuleFor(x => x.Estimate).InclusiveBetween(1, 21);
    }
  }
}
