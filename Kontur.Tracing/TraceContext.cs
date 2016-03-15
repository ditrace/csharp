using System;
using JetBrains.Annotations;
using Kontur.Tracing.Core;

namespace Kontur.Tracing
{
    public class TraceContext : ITraceContextAnnotator
    {
        [NotNull]
        public static TraceContext Current
        {
            get { return new TraceContext(Core.TraceContext.Current); }
        }

        private TraceContext([NotNull] ITraceContextAnnotator traceContextAnnotator)
        {
            this.traceContextAnnotator = traceContextAnnotator;
        }

        [NotNull]
        public string TraceId
        {
            get { return traceContextAnnotator.TraceId; }
        }

        [NotNull]
        public string ContextId
        {
            get { return traceContextAnnotator.ContextId; }
        }

        [CanBeNull]
        public string ParentContextId
        {
            get { return traceContextAnnotator.ParentContextId; }
        }

        public bool IsActive
        {
            get { return traceContextAnnotator.IsActive; }
        }

        public void RecordTimepoint(Timepoint type)
        {
            traceContextAnnotator.RecordTimepoint(type);
        }

        public void RecordTimepoint(Timepoint type, DateTime time)
        {
            traceContextAnnotator.RecordTimepoint(type, time);
        }

        public void RecordAnnotation(Annotation type, string value)
        {
            traceContextAnnotator.RecordAnnotation(type, value);
        }

        public void RecordAnnotation(string name, string value)
        {
            traceContextAnnotator.RecordAnnotation(name, value);
        }

        private readonly ITraceContextAnnotator traceContextAnnotator;
    }
}