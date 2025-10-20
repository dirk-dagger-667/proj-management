using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain;
using proj_mngmt_api.Domain.Exteinsions;
using proj_mngmt_api.Infrastructure;
using proj_mngmt_api.Infrastructure.Data;

namespace proj_mngmt_api.Features.ProjectsManagement.GetTasks
{
  public record TaskListItem(Guid Id, string Title, Status Status);

  public record GetTasksResponse(int TotalCount, IEnumerable<TaskListItem> Tasks);

  public class GetTasksEndpoint : IEndpoint
  {
    public void MapEnpoint(WebApplication app)
      => app.MapGet("api/projects/{projectId}/tasks", Handle)
      .WithTags("Projects")
      .AddEndpointFilter<ValidationFilter<PaganationParameters>>();

    private static async Task<Results<
      Ok<GetTasksResponse>,
      ValidationProblem,
      ProblemHttpResult>>
      Handle([FromRoute] Guid projectId,
      HttpContext httpContext,
      ProjMngtDbContext dbContext,
      [AsParameters] PaganationParameters pageParams)
    {
      GetTasksResponse response;

      var query = dbContext.Tasks
        .AsNoTracking()
        .Where(t => t.ProjectId == projectId);

      var count = 0;

      if (string.IsNullOrWhiteSpace(pageParams.filter))
      {
        var taskListItems = await query
        .OrderBy(t => t.CreatedAt)
        .Skip((pageParams.Page - 1) * pageParams.Size)
        .Take(pageParams.Size)
        .Select(task => task.ToListItem())
        .ToListAsync();

        count = await query.CountAsync();
        response = new GetTasksResponse(count, taskListItems);
      }
      else
      {
        var taskListItems = await query
        .Where(t => t.Title.ToLower().Contains(pageParams.filter.ToLower()) ||
        (t.Description != null && t.Description.ToLower().Contains(pageParams.filter.ToLower())))
        .OrderBy(t => t.CreatedAt)
        .Skip((pageParams.Page - 1) * pageParams.Size)
        .Take(pageParams.Size)
        .Select(task => task.ToListItem())
        .ToListAsync();

        count = await query.CountAsync();
        response = new GetTasksResponse(count, taskListItems);
      }

      return TypedResults.Ok(response);
    }
  }
}
