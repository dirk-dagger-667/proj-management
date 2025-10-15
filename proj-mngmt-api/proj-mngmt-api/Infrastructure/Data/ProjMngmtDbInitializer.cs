using Bogus;
using Microsoft.EntityFrameworkCore;
using proj_mngmt_api.Domain;

namespace proj_mngmt_api.Infrastructure.Data
{
  public static class ProjMngmtDbInitializer
  {
    public static async Task SeedAsync(ProjMngtDbContext context, CancellationToken cancellationToken = default)
    {
      await context.Database.MigrateAsync(cancellationToken);
      await SeedProjectsAsync(context, cancellationToken);
    }

    private static async Task SeedProjectsAsync(ProjMngtDbContext context, CancellationToken cancellationToken)
    {
      if (!await context.Projects.AnyAsync(cancellationToken))
      {
        context.Projects.AddRange(new FakeDataDbGenerator().GenerateProjects(6));
        await context.SaveChangesAsync(cancellationToken);
      }
    }

    private class FakeDataDbGenerator
    {
      public IEnumerable<Project> GenerateProjects(int count)
      {
        var auditFaker = new Faker<AuditEntry>()
          .RuleFor(audit => audit.CreatedAt, f => f.Date.Past(3))
          .RuleFor(audit => audit.Metadata, f => f.Random.Words(10));

        var taskFaker = new Faker<TaskItem>()
          .RuleFor(task => task.Assignee, f => f.Name.FullName())
          .RuleFor(task => task.Description, f => f.Random.Words(10))
          .RuleFor(task => task.Title, f => f.Random.Words(3))
          .RuleFor(task => task.Estimate, f => f.Random.Int(1, 17))
          .RuleFor(task => task.CreatedAt, f => f.Date.Past(3))
          .RuleFor(task => task.Priority, f => f.PickRandom<Priority>())
          .RuleFor(task => task.Status, f => f.PickRandom<Status>())
          .RuleFor(task => task.Type, f => f.PickRandom<TaskType>())
          .RuleFor(task => task.ConcurrencyToken, f => f.Random.Guid())
          .FinishWith((f, t) =>
            auditFaker.GenerateBetween(20, 100).ForEach(a => t.AuditEntries.Add(a)));

        var projectFaker = new Faker<Project>()
           .RuleFor(proj => proj.Title, f => f.Company.CompanyName())
           .FinishWith((f, p) =>
             taskFaker.GenerateBetween(20, 100).ForEach(t => p.Tasks.Add(t)));

        return projectFaker.Generate(count);
      }
    }
  }
}
