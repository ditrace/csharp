using JetBrains.Annotations;
using Kontur.Tracing.Core.Impl;

namespace Kontur.Tracing.Core
{
    public static class TraceContext
    {
        [NotNull]
        public static ITraceContextAnnotator Current
        {
            get
            {
                var currentAnnotator = Trace.tracingAnnotatorContext.Value;
                return currentAnnotator ?? NoOpTraceContext.Instance;
            }
        }
    }
}