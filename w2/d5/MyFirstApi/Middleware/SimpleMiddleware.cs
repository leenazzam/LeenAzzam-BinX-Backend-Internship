public class SimpleMiddleware
{
    private readonly RequestDelegate _next;

    public SimpleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

        await _next(context);
    }
}