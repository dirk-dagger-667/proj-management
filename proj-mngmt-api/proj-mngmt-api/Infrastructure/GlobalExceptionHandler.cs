using Microsoft.AspNetCore.Diagnostics;
using proj_mngmt_api.Domain.Exteinsions;

namespace proj_mngmt_api.Infrastructure
{
  public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
  {
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
      httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

      var pdContext = new ProblemDetailsContext
      {
        HttpContext = httpContext,
        Exception = exception,
      };

      pdContext.ProblemDetails
        .WithType(exception.GetType().Name)
        .WithTitle("Something went wrong. Please try again later.")
        .WithDetail("A server operation failed. An exception was thrown.");

      return await problemDetailsService.TryWriteAsync(pdContext);
    }
  }
}
