namespace Kontur.Tracing
{
    public static class TraceHttpHeaders
    {
        public const string XKonturTraceId = Core.TraceHttpHeaders.XKonturTraceId;
        public const string XKonturTraceSpanId = Core.TraceHttpHeaders.XKonturTraceContextId;
        public const string XKonturTraceIsSampled = Core.TraceHttpHeaders.XKonturTraceIsActive;
        public const string XKonturTraceParentSpanId = Core.TraceHttpHeaders.XKonturTraceParentContextId;
        public const string XKonturTraceProfileId = "X-Kontur-Trace-ProfileId";
    }
}