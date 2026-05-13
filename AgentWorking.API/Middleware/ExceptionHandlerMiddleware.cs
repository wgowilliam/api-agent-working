using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AgentWorking.API.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/problem+json";
            var problem = new ValidationProblemDetails
            {
                Type = "validation_error",
                Title = "One or more validation errors occurred.",
                Status = 400
            };
            foreach (var failure in ex.Errors)
                problem.Errors.TryAdd(failure.PropertyName, [failure.ErrorMessage]);

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "not_found", Title = ex.Message, Status = 404
            }));
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "unauthorized", Title = "Credenciais inválidas.", Status = 401
            }));
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = 409;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "conflict", Title = ex.Message, Status = 409
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Type = "internal_error",
                Title = "An unexpected error occurred.",
                Status = 500
            }));
        }
    }
}
