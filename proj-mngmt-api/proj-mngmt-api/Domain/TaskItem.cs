namespace proj_mngmt_api.Domain
{
  public enum TaskType { Story, Bug }
  public enum Priority { Low, Normal, High, Critical }
  public enum Status { ToDo, InProgress, ReadyForTest, Done }

  public class TaskItem
  {
    public Guid Id { get; set; }
    public TaskType Type { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Assignee { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public int Estimate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid ConcurrencyToken { get; set; }

    public Guid ProjectId { get; set; }
    public required Project Project { get; set; }

    public List<AuditEntry> AuditEntries { get; set; } = new();
  }
}
