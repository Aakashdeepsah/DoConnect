// Middleware/ExceptionMiddleware.cs
using System.Net;
using System.Text.Json;

namespace DoConnect.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next; _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found");
                await Write(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized");
                await Write(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Bad request");
                await Write(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await Write(context, HttpStatusCode.InternalServerError,
                    "Something went wrong. Please try again later.");
            }
        }

        private static async Task Write(HttpContext ctx, HttpStatusCode code, string msg)
        {
            ctx.Response.StatusCode  = (int)code;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = (int)code,
                Message    = msg,
                Timestamp  = DateTime.UtcNow
            }));
        }
    }
}
