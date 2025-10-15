using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Infrastructure;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.ProjectsManagement.Tasks
{
  public class PutTaskEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
      => app.MapPut("api/tasks/{taskId}", Handle)
      .WithTags("Tasks")
      .AddEndpointFilter<ValidationFilter<TaskItemDto>>();

    private static async Task<Results<
      NoContent,
      Conflict<string>,
      ValidationProblem>> Handle(
      [FromRoute] Guid taskId,
      [FromBody] TaskItemDto taskItemDto,
      HttpContext httpContext,
      ProjMngtDbContext dbContext)
    {
      var result = await dbContext.Tasks
        .Where(t => t.Id == taskId && t.ConcurrencyToken == taskItemDto.ConcurrencyToken)
        .ExecuteUpdateAsync(setters => setters
        .SetProperty(t => t.Assignee, taskItemDto.Assignee)
        .SetProperty(t => t.Description, taskItemDto.Description)
        .SetProperty(t => t.Title, taskItemDto.Title)
        .SetProperty(t => t.Status, taskItemDto.Status)
        .SetProperty(t => t.Priority, taskItemDto.Priority)
        .SetProperty(t => t.Type, taskItemDto.Type)
        .SetProperty(t => t.CreatedAt, DateTimeOffset.Now)
        .SetProperty(t => t.Estimate, taskItemDto.Estimate)
        .SetProperty(t => t.ConcurrencyToken, Guid.NewGuid()));

      if (result == 0)
        return TypedResults.Conflict("Something went wrong, please try again.");

      return TypedResults.NoContent();
    }
  }
}
