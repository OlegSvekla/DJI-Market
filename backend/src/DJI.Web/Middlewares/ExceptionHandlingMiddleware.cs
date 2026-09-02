using System.Text.Json;
using DJI.Core.Constants;
using DJI.Core.Exceptions;

namespace DJI.Web.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private const string ProblemContentType = "application/problem+json";

    private const string ProblemType = "about:blank";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainValidationException exception)
        {
            await WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                ErrorMessages.BadRequestTitle,
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled error while processing {Path}.", context.Request.Path);

            await WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ErrorMessages.ServerErrorTitle,
                ErrorMessages.ServerErrorDetail);
        }
    }

    private static async Task WriteAsync(HttpContext context, int status, string title, string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = ProblemContentType;

        var problem = new
        {
            type = ProblemType,
            title,
            status,
            detail,
            instance = context.Request.Path.Value,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
