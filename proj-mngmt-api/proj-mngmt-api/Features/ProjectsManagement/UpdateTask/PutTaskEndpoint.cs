using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Features.ProjectsManagement.Tasks;
using proj_mngmt_api.Infrastructure;
using proj_mngmt_api.Infrastructure.Data;
using System.Text.Json;

namespace proj_mngmt_api.Features.ProjectsManagement.UpdateTask
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
      ValidationProblem,
      ProblemHttpResult>> Handle(
      [FromRoute] Guid taskId,
      [FromBody] TaskItemDto taskItemDto,
      ProjMngtDbContext dbContext)
    {
      await using var transaction = await dbContext.Database.BeginTransactionAsync();

      try
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
        .SetProperty(t => t.CreatedAt, DateTime.UtcNow)
        .SetProperty(t => t.Estimate, taskItemDto.Estimate)
        .SetProperty(t => t.ConcurrencyToken, Guid.NewGuid()));

        if (result == 0)
        {
          await transaction.RollbackAsync();
          return TypedResults.Conflict("Something went wrong, please try again.");
        }  

        var addAuditResult = dbContext.Audits.Add(new Domain.AuditEntry
        {
          Metadata = JsonSerializer.Serialize(taskItemDto,
          new JsonSerializerOptions { WriteIndented = true }),

          CreatedAt = DateTime.UtcNow,
          TaskId = taskId,
        });

        if(addAuditResult is null)
        {
          await transaction.RollbackAsync();
          return TypedResults.Problem();
        }

        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
  }
}
