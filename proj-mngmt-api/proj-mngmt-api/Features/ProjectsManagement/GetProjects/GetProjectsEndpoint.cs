using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain.Exteinsions;
using proj_mngmt_api.Infrastructure;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.Projects.GetProjects
{
  public record ProjectDto(Guid Id, string Title) { }

  public class GetProjectsEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
        => app.MapGet("api/projects", Handle)
      .WithTags("Projects")
      .AddEndpointFilter<ValidationFilter<PaganationParameters>>();

    private static async Task<Results<
      Ok<IEnumerable<ProjectDto>>,
      ValidationProblem,
      ProblemHttpResult>>
      Handle(ProjMngtDbContext dbContext,
      [AsParameters] PaganationParameters pageParams)
    {
      var projectDtos = await dbContext.Projects
        .AsNoTracking()
        .OrderBy(p => p.Title)
        .Skip((pageParams.Page - 1) * pageParams.Size)
        .Take(pageParams.Size)
        .Select(p => p.ToDto())
        .ToListAsync();

      return TypedResults.Ok<IEnumerable<ProjectDto>>(projectDtos);
    }
  }
}
