using System;
using System.Collections.Specialized;
using Kontur.Tracing.Core.Logging;

namespace Kontur.Tracing
{
    public static class TraceDataExtractor
    {
        public static void ExtractFromHttpHeaders(
            NameValueCollection headers,
            out string traceId,
            out string traceSpanId,
            out string traceParentSpanId,
            out string traceProfileId,
            out bool? traceSampled,
            ILog log)
        {
            traceId = null;
            traceSpanId = null;
            traceParentSpanId = null;
            traceProfileId = null;
            traceSampled = null;

            try
            {
                ExtractFromHttpHeaders(headers, out traceId, out traceSpanId, out traceParentSpanId, out traceProfileId, out traceSampled);
            }
            catch (Exception error)
            {
                log.Error("Error in extracting trace data from headers: " + error);
            }
        }

        public static void ExtractFromHttpHeaders(
            NameValueCollection headers,
            out string traceId,
            out string traceSpanId,
            out string traceParentSpanId,
            out string traceProfileId,
            out bool? traceSampled)
        {
            traceSpanId = null;
            traceParentSpanId = null;
            traceProfileId = null;
            traceSampled = null;

            traceId = headers[TraceHttpHeaders.XKonturTraceId];

            if (String.IsNullOrEmpty(traceId))
                return;
            traceSpanId = headers[TraceHttpHeaders.XKonturTraceSpanId];
            traceParentSpanId = headers[TraceHttpHeaders.XKonturTraceParentSpanId];
            traceProfileId = headers[TraceHttpHeaders.XKonturTraceProfileId];
            traceSampled = !String.IsNullOrEmpty(headers[TraceHttpHeaders.XKonturTraceIsSampled]);
        }
    }
}