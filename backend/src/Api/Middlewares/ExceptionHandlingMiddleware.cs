using InsurancePolicyManager.Application.Common;
using InsurancePolicyManager.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace InsurancePolicyManager.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (DomainException ex)
    {
      _logger.LogWarning(ex, "Erro de domínio: {Message}", ex.Message);
      await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erro não tratado");
      await WriteResponse(context, HttpStatusCode.InternalServerError, "Erro interno no servidor.");
    }
  }

  private static Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)statusCode;

    var response = ApiResponse<object>.Fail(message);
    return context.Response.WriteAsync(JsonSerializer.Serialize(response));
  }
}
