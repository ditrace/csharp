using System.Runtime.Remoting.Messaging;
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
                var currentAnnotator = (ITraceContextAnnotator)CallContext.LogicalGetData(Trace.AnnotatorStorageKey);
                return currentAnnotator ?? NoOpTraceContext.Instance;
            }
        }
    }
}