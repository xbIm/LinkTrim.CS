using Serilog.Core;
using Serilog.Events;

namespace LinkTrim.Web.Middleware;

public class HttpRequestTraceIdEnricher(IHttpContextAccessor contextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        var traceId = contextAccessor.HttpContext?.TraceIdentifier;

        if (traceId != null)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", traceId, true));
        }
    }
}