namespace Kontur.Tracing.Core
{
    public static class TraceHttpHeaders
    {
        public const string XKonturTraceId = "X-Kontur-Trace-Id";
        public const string XKonturTraceContextId = "X-Kontur-Trace-SpanId";
        public const string XKonturTraceParentContextId = "X-Kontur-Trace-ParentSpanId";
        public const string XKonturTraceIsActive = "X-Kontur-Trace-IsSampled";
    }
}