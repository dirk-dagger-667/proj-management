using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.ProjectsManagement.AuditHistory
{
  public record AuditEntryDto(Guid Id,
    string Metadata,
    DateTime CreatedAt);

  public class GetAuditsEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
      => app.MapGet("api/tasks/{taskId}/audits", Handle).WithTags("Tasks");

    private static async Task<Results<
      Ok<IEnumerable<AuditEntryDto>>,
        ValidationProblem>> Handle([FromRoute] Guid taskId,
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

      var auditsDtos = await dbContext.Audits
        .AsNoTracking()
        .Where(ae => ae.TaskId == taskId)
        .OrderBy(ae => ae.CreatedAt)
        .Skip((page - 1) * limit)
        .Take(limit)
        .Select(ae => new AuditEntryDto(ae.Id, ae.Metadata, ae.CreatedAt))
        .ToListAsync();

      return TypedResults.Ok<IEnumerable<AuditEntryDto>>(auditsDtos);
    }
  }
}
