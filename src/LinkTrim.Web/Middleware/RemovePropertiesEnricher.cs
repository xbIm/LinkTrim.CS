using Serilog.Core;
using Serilog.Events;

namespace LinkTrim.Web.Middleware;

public class RemovePropertiesEnricher(string[] toRemove) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        foreach (var key in toRemove)
        {
            logEvent.RemovePropertyIfPresent(key);
        }
    }
}