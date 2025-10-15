using proj_mngmt_api.Domain;

namespace proj_mngmt_api.Features.ProjectsManagement.Tasks
{
  public record TaskItemDto(
    Guid Id,
    Guid ProjectId,
    TaskType Type,
    string Title,
    string? Description,
    string? Assignee,
    Priority Priority,
    Status Status,
    int Estimate,
    DateTime? CreatedAt,
    Guid ConcurrencyToken);
}
