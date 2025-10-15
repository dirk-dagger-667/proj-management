using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain;
using proj_mngmt_api.Domain.Exteinsions;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.ProjectsManagement.Tasks
{
  public record TaskListItem(Guid Id ,string Title, Status Status) { }

  public class GetTasksEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
      => app.MapGet("api/projects/{projectId}/tasks", Handle).WithTags("Projects");

    private static async Task<Results<
      Ok<IEnumerable<TaskListItem>>,
      ValidationProblem>>
      Handle([FromRoute] Guid projectId,
      HttpContext httpContext,
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

      var taskListItems = await dbContext.Tasks
        .AsNoTracking()
        .Where(t => t.ProjectId == projectId)
        .OrderBy(t => t.CreatedAt)
        .Skip((page - 1) * limit)
        .Take(limit)
        .Select(task => task.ToListItem())
        .ToListAsync();

      return TypedResults.Ok<IEnumerable<TaskListItem>>(taskListItems);
    }
  }
}
