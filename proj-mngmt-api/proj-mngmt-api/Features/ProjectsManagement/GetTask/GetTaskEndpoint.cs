using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain.Exteinsions;
using proj_mngmt_api.Features.ProjectsManagement.Tasks;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.ProjectsManagement.GetTask
{
  public class GetTaskEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
      => app.MapGet("api/tasks/{id}", Handle).WithTags("Tasks");

    private static async Task<Results<Ok<TaskItemDto>, NotFound, ProblemHttpResult>> Handle(
      [FromRoute] Guid id,
      HttpContext httpContext,
      ProjMngtDbContext dbContext)
    {
      var entity = await dbContext.Tasks
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id);

      if (entity is null)
        return TypedResults.NotFound();

      return TypedResults.Ok(entity.ToDto());
    }
  }
}
