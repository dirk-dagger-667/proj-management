namespace proj_mngmt_api.Domain
{
  public class AuditEntry
  {
    public Guid Id { get; set; }
    public required string Metadata { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid TaskId { get; set; }
    public required TaskItem Task { get; set; }
  }
}
