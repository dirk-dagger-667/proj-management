using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace proj_mngmt_api.Domain.Exteinsions
{
  public static class ProblemDetailsExtensions
  {
    public static ProblemDetails WithTraceId(this ProblemDetails problemDetails, string? traceId)
    {
      if (!problemDetails.Extensions.ContainsKey("traceId"))
        problemDetails.Extensions.Add("traceId", traceId);

      return problemDetails;
    }

    public static ProblemDetails WithStatus(this ProblemDetails problemDetails, int? statusCode)
    {
      problemDetails.Status = statusCode;

      return problemDetails;
    }

    public static ProblemDetails WithTraceId(this ProblemDetails problemDetails, HttpContext? httpContext)
    {
      problemDetails.WithTraceId(httpContext?.Features.Get<IHttpActivityFeature>()?.Activity?.Id);

      return problemDetails;
    }

    public static ProblemDetails WithInstance(this ProblemDetails problemDetails, HttpContext? httpContext)
    {
      problemDetails.WithInstance($"{httpContext?.Request.Method} {httpContext?.Request.Path}");

      return problemDetails;
    }

    public static ProblemDetails WithInstance(this ProblemDetails problemDetails, string? instance)
    {
      problemDetails.Instance = instance;

      return problemDetails;
    }

    public static ProblemDetails WithRequestId(this ProblemDetails problemDetails, string? requestId)
    {
      problemDetails.Extensions.Add("requestId", requestId);

      return problemDetails;
    }

    public static ProblemDetails WithRequestId(this ProblemDetails problemDetails, HttpContext? httpContext)
    {
      problemDetails.WithRequestId(httpContext?.TraceIdentifier);

      return problemDetails;
    }

    public static ProblemDetails WithType(this ProblemDetails problemDetails, string? type)
    {
      problemDetails.Type = type;

      return problemDetails;
    }

    public static ProblemDetails WithTitle(this ProblemDetails problemDetails, string? title)
    {
      problemDetails.Title = title;

      return problemDetails;
    }

    public static ProblemDetails WithDetail(this ProblemDetails problemDetails, string? detail)
    {
      problemDetails.Detail = detail;

      return problemDetails;
    }
  }
}
