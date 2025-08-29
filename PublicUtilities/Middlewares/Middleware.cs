using System.Text.Json;

namespace PublicUtilities.Middlewares;

public class MiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MiddleWare> _logger;

    public MiddleWare(RequestDelegate next, ILogger<MiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, $"Ошибка пользователя");
            await ProcessExceptionAsync(context, ex.Message, ex.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение");
            await ProcessExceptionAsync(context, "Внутрення ошибка сервера", StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task ProcessExceptionAsync(HttpContext context, string message, int code)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            message
        }));
    }
}
