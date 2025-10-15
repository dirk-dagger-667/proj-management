using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using proj_mngmt_api.Features;
using proj_mngmt_api.Features.ProjectsManagement.Tasks;
using proj_mngmt_api.Infrastructure.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProjMngtDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

if(builder.Environment.IsDevelopment())
{
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
}

ServiceDescriptor[] serviceDescriptors =Assembly.GetExecutingAssembly().DefinedTypes
  .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && t.IsClass && !t.IsAbstract)
  .Select(t => ServiceDescriptor.Transient(typeof(IEndpoint), t))
  .ToArray();

builder.Services.TryAddEnumerable(serviceDescriptors);
builder.Services.AddValidatorsFromAssemblyContaining<TaskItemValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.

await using(var scope = app.Services.CreateAsyncScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<ProjMngtDbContext>();
  await ProjMngmtDbInitializer.SeedAsync(dbContext);
}

using (var scope = app.Services.CreateScope())
{
  foreach (var endpoint in scope.ServiceProvider.GetServices<IEndpoint>())
  {
    endpoint.MapEnpoint(app);
  }
}

if(app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.Run();






