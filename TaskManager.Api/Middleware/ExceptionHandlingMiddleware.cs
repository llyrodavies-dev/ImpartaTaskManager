using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using TaskManager.Application.Exceptions;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Api.Middleware
{
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context,ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            (HttpStatusCode statusCode, object problemDetails) = exception switch
            {
                FluentValidation.ValidationException validationEx =>
                    (HttpStatusCode.BadRequest, CreateValidationErrorResponse(validationEx, context)),

                AuthenticationException auth =>
                    (HttpStatusCode.Unauthorized, CreateProblemDetails(HttpStatusCode.Unauthorized, "Authentication Failed", auth.Message, context)),

                UnauthorizedAccessException _ =>
                    (HttpStatusCode.Forbidden, CreateProblemDetails(HttpStatusCode.Forbidden, "Access Denied", "You don't have permission to access this resource.", context)),

                DomainException domain =>
                    (HttpStatusCode.BadRequest, CreateProblemDetails(HttpStatusCode.BadRequest, "Business Rule Violation", domain.Message, context)),

                NotFoundException notFound =>
                    (HttpStatusCode.NotFound, CreateProblemDetails(HttpStatusCode.NotFound, "Resource Not Found", notFound.Message, context)),

                _ => (HttpStatusCode.InternalServerError, CreateProblemDetails(HttpStatusCode.InternalServerError, "An unexpected error occurred", "An error occurred processing your request.", context))
            };

            _logger.LogError(exception, "Exception occurred: {ExceptionType}", exception.GetType().Name);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }

        private static object CreateProblemDetails(HttpStatusCode statusCode, string title, string detail, HttpContext context)
        {
            return new
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path.ToString()
            };
        }

        private static object CreateValidationErrorResponse(FluentValidation.ValidationException validationException, HttpContext context)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = errors,
                Instance = context.Request.Path.ToString()
            };
        }
    }
}
