using proj_mngmt_api.Features.ProjectsManagement.Tasks;

namespace proj_mngmt_api.Domain.Exteinsions
{
  public static class TaskItemExtensions
  {
    public static TaskItemDto ToDto(this TaskItem task)
    {
      if (task == null) throw new ArgumentNullException(nameof(task));

      return new TaskItemDto(
          Id: task.Id,
          ProjectId: task.ProjectId,
          Type: task.Type,
          Title: task.Title,
          Description: task.Description,
          Assignee: task.Assignee,
          Priority: task.Priority,
          Status: task.Status,
          Estimate: task.Estimate,
          CreatedAt: task.CreatedAt,
          ConcurrencyToken: task.ConcurrencyToken
      );
    }

    public static TaskListItem ToListItem(this TaskItem task)
      => new TaskListItem(
          Id: task.Id,
          Title: task.Title,
          Status: task.Status);
  }
}
