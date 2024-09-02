namespace LinkTrim.Web.Middleware;

public class TraceIdMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.TraceIdentifier = Guid.NewGuid().ToString().Replace("-", "");
        context.Response.Headers["X-Trace-Id"] = context.TraceIdentifier;
        await next(context);
    }
}