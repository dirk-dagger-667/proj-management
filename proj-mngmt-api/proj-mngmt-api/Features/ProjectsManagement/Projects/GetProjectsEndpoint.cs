using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain.Exteinsions;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.Projects.Projects
{
  public record ProjectDto(Guid Id, string Title) { }

  public class GetProjectsEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
        => app.MapGet("api/projects", Handle).WithTags("Projects");

    private static async Task<Results<
      Ok<IEnumerable<ProjectDto>>,
      ValidationProblem>>
      Handle(HttpContext httpContext,
      ProjMngtDbContext dbContext,
      [FromQuery] int page = 1)
    {
      var path = httpContext.Request.Path;
      var traceId = httpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id;
      var limit = 50;

      var extensions = new Dictionary<string, object?>()
            {
                { "traceId", traceId }
            };

      //if (page is null || page! < 1)
        if (page < 1)
        {
        return TypedResults.ValidationProblem(
            new Dictionary<string, string[]>()
            { { "Invalid argument/s", new[] { $"Page value is less than 1." } } },
            "Invalid argument for validation",
            $"{httpContext?.Request.Method} {path}",
            "One or more validation errors occurred.",
            "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            extensions);
      }

      var projectDtos = await dbContext.Projects
        .AsNoTracking()
        .OrderBy(p => p.Title)
        .Skip((page - 1) * limit)
        .Take(limit)
        .Select(p => p.ToDto())
        .ToListAsync();

      return TypedResults.Ok<IEnumerable<ProjectDto>>(projectDtos);
    }
  }
}
