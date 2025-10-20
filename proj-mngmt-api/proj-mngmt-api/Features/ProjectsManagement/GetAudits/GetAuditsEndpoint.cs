using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain;
using proj_mngmt_api.Infrastructure;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.ProjectsManagement.GetAudits
{
  public record AuditEntryDto(Guid Id,
    AuditType AuditType,
    string Metadata,
    DateTime CreatedAt);

  public class GetAuditsEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
      => app.MapGet("api/tasks/{taskId}/audits", Handle)
      .WithTags("Tasks")
      .AddEndpointFilter<ValidationFilter<PaganationParameters>>();

    private static async Task<Results<
      Ok<IEnumerable<AuditEntryDto>>,
        ValidationProblem>> Handle([FromRoute] Guid taskId,
      ProjMngtDbContext dbContext,
      [AsParameters] PaganationParameters pageParams)
    {
      var auditsDtos = await dbContext.Audits
        .AsNoTracking()
        .Where(ae => ae.TaskId == taskId)
        .OrderBy(ae => ae.CreatedAt)
        .Skip((pageParams.Page - 1) * pageParams.Size)
        .Take(pageParams.Size)
        .Select(ae => new AuditEntryDto(ae.Id, AuditType.Updated, ae.Metadata, ae.CreatedAt))
        .ToListAsync();

      return TypedResults.Ok<IEnumerable<AuditEntryDto>>(auditsDtos);
    }
  }
}
