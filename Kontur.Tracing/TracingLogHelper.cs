using Kontur.Tracing.Core;

namespace Kontur.Tracing
{
    public static class TracingLogHelper
    {
        public static string CreateLogPrefix(ITraceContextAnnotator context)
        {
            if (context == null)
                return string.Empty;

            var traceId = context.TraceId;
            if (traceId.Length > 8)
                traceId = traceId.Substring(0, 8);

            return context.IsActive
                ? "T-" + traceId + "(+)"
                : "T-" + traceId + "(-)";
        }
    }
}