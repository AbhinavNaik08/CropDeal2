using System.Net;
using System.Text.Json;
using CropDeal.Exceptions;

namespace CropDeal.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (NotFoundException ex)
            {
                await HandleException(context, ex, HttpStatusCode.NotFound, logAsError: false);
            }
            catch (BadRequestException ex)
            {
                await HandleException(context, ex, HttpStatusCode.BadRequest, logAsError: false);
            }
            catch (UnauthorizedException ex)
            {
                await HandleException(context, ex, HttpStatusCode.Unauthorized, logAsError: false);
            }
            catch (ForbiddenException ex)
            {
                await HandleException(context, ex, HttpStatusCode.Forbidden, logAsError: false);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex, HttpStatusCode.InternalServerError, logAsError: true);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex, HttpStatusCode statusCode, bool logAsError)
        {
            if (logAsError)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("{ExceptionType} occurred while processing {Method} {Path}: {Message}",
                    ex.GetType().Name, context.Request.Method, context.Request.Path, ex.Message);
            }

            var message = logAsError ? "An unexpected error occurred. Please try again later." : ex.Message;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                message,
                statusCode = context.Response.StatusCode,
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}