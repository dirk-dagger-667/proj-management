using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using proj_mngmt_api.Domain.Exteinsions;

namespace proj_mngmt_api.Infrastructure
{
  public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
  {
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
      var pdContext = BuildPDContext(httpContext, exception);

      var acceptHeader = httpContext.Request.Headers["Accept"].Any()
        ? httpContext.Request.Headers["Accept"].FirstOrDefault()
        : null;

      if (!string.IsNullOrWhiteSpace(acceptHeader)
        && (acceptHeader.Contains("/xml") || acceptHeader.Contains("+xml")))
      {
        pdContext.ProblemDetails
          .WithInstance(httpContext)
          .WithRequestId(httpContext)
          .WithTraceId(httpContext);

        var result = new ObjectResult(pdContext.ProblemDetails)
        {
          StatusCode = httpContext.Response.StatusCode,
          ContentTypes = { "application/problem+xml", "application/xml", "text/xml" },
        };

        await result.ExecuteResultAsync(new ActionContext { HttpContext = httpContext });

        return true;
      }

      return await problemDetailsService.TryWriteAsync(pdContext);
    }

    private ProblemDetailsContext BuildPDContext(HttpContext httpContext, Exception exception)
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

      return pdContext;
    }
  }
}
