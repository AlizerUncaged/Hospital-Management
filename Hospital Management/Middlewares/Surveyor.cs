namespace Hospital_Management.Middlewares;

public class Surveyor
{
    private readonly RequestDelegate _next;

    public Surveyor(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get User-Agent.
        if (context.Request.Headers.TryGetValue("user-agent", out var userAgent) &&
            !string.IsNullOrWhiteSpace(userAgent))
        {
            if (userAgent.ToString().Contains("eye"))
            {
                context.Request.HttpContext.Response.Redirect("/error");
                return;
            }
        }

        await _next(context);
    }
}