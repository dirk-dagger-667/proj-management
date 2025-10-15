using proj_mngmt_api.Features.Projects.Projects;

namespace proj_mngmt_api.Domain.Exteinsions
{
  public static class ProjectExtensions
  {
    public static ProjectDto ToDto(this Project project)
      => new ProjectDto(project.Id, project.Title);
  }
}
