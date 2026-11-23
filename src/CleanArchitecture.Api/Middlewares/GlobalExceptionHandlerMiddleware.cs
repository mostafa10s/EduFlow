
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Models;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CleanArchitecture.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;

            var correlationId =
                context.Response.Headers["X-Correlation-ID"].ToString();

            var (statusCode, message, errors) = HandleException(ex);

            if (statusCode >= 500)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception. CorrelationId: {CorrelationId}",
                    correlationId);
            }
            else
            {
                _logger.LogWarning(
                    "Request failed with status {StatusCode}. Message: {Message}. CorrelationId: {CorrelationId}",
                    statusCode,
                    message,
                    correlationId);
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = OperationResult.Failure(
                errors,
                statusCode,
                message);

            var responseJson = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(responseJson);
        }
    }

    private static (
        int StatusCode,
        string Message,
        IDictionary<string, string[]>? Errors
    ) HandleException(Exception exception)
    {
        return exception switch
        {
            FluentValidation.ValidationException validationException =>
                (
                    StatusCodes.Status422UnprocessableEntity,
                    "Validation failed.",
                    validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(e => e.ErrorMessage).ToArray())
                ),

            NotFoundException notFoundException =>
                (
                    StatusCodes.Status404NotFound,
                    notFoundException.Message,
                    null
                ),

            BadRequestException badRequestException =>
                (
                    StatusCodes.Status400BadRequest,
                    badRequestException.Message,
                    null
                ),

            UnauthorizedException unauthorizedException =>
                (
                    StatusCodes.Status401Unauthorized,
                    unauthorizedException.Message,
                    null
                ),

            ForbiddenException forbiddenException =>
                (
                    StatusCodes.Status403Forbidden,
                    forbiddenException.Message,
                    null
                ),

            ConflictException conflictException =>
                (
                    StatusCodes.Status409Conflict,
                    conflictException.Message,
                    null
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    null
                )
        };
    }
}
