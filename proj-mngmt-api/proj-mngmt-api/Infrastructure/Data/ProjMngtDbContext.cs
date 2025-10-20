using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain;

namespace proj_mngmt_api.Infrastructure.Data
{
  public class ProjMngtDbContext : DbContext
  {
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<AuditEntry> Audits { get; set; }

    public ProjMngtDbContext(DbContextOptions<ProjMngtDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder
      .Entity<Project>(project =>
      {
        project.HasKey(p => p.Id);

        project.Property(p => p.Title)
        .IsRequired()
        .HasMaxLength(50);

        project.HasMany(b => b.Tasks)
        .WithOne(t => t.Project)
        .HasForeignKey(t => t.ProjectId)
        .IsRequired();
      });

      modelBuilder.Entity<TaskItem>(task =>
      {
        task.HasKey(t => t.Id);

        task.HasIndex(t => t.CreatedAt).IsDescending();

        task.Property(t => t.Title).IsRequired().HasMaxLength(30);
        task.Property(t => t.Description).HasMaxLength(500);
        task.Property(t => t.Type).IsRequired();
        task.Property(t => t.Assignee).HasMaxLength(20);
        task.Property(t => t.Priority).IsRequired();
        task.Property(t => t.Status).IsRequired();

        task.HasMany(t => t.AuditEntries)
        .WithOne(ae => ae.Task)
        .HasForeignKey(ae => ae.TaskId)
        .IsRequired();
      });

      modelBuilder.Entity<AuditEntry>(auditEntry => 
      {
        auditEntry.HasKey(t => t.Id);

        auditEntry.HasIndex(t => t.CreatedAt).IsDescending();

        auditEntry.Property(ae => ae.Metadata).IsRequired().HasMaxLength(100);
        auditEntry.Property(ae => ae.CreatedAt).IsRequired();
      });
    }
  }
}
