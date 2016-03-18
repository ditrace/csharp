using System.Net;

namespace Kontur.Tracing.Core
{
    public static class RequestExtensions
    {
        public static void SetTracingHeaders(this WebRequest request, ITraceContext context)
        {
            request.Headers[Tracing.TraceHttpHeaders.XKonturTraceId] = context.TraceId;
            request.Headers[Tracing.TraceHttpHeaders.XKonturTraceSpanId] = context.ContextId;
            request.Headers[Tracing.TraceHttpHeaders.XKonturTraceIsSampled] = context.IsActive.ToString();
        }
        public static void ExtractFromHttpHeaders(WebHeaderCollection headers, out string traceId, out string traceSpanId, out bool? traceSampled)
        {
            traceSpanId = null;
            traceSampled = null;

            traceId = headers[Tracing.TraceHttpHeaders.XKonturTraceId];

            if (string.IsNullOrEmpty(traceId))
                return;
            traceSpanId = headers[Tracing.TraceHttpHeaders.XKonturTraceSpanId];
            traceSampled = headers[Tracing.TraceHttpHeaders.XKonturTraceIsSampled].ToLower() == "true";
        }
    }
}