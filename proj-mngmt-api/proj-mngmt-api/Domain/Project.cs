namespace proj_mngmt_api.Domain
{
  public class Project
  {
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public List<TaskItem> Tasks { get; set; } = new();
  }
}
