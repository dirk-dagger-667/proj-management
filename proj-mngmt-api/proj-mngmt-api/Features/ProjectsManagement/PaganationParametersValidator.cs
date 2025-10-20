using FluentValidation;

namespace proj_mngmt_api.Features.ProjectsManagement
{
  public class PaganationParametersValidator : AbstractValidator<PaganationParameters>
  {
    public PaganationParametersValidator()
    {
      RuleFor(x => x.Page).NotNull().GreaterThan(-1);
      RuleFor(x => x.Size).NotNull().GreaterThan(0).LessThan(100);
    }
  }
}
