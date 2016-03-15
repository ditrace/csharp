using System;
using JetBrains.Annotations;
using Kontur.Tracing.Core;

namespace Kontur.Tracing
{
    public abstract class RpcTraceContextBase : IRpcTraceContext
    {
        protected RpcTraceContextBase([NotNull] ITraceContext traceContext)
        {
            this.traceContext = traceContext;
        }

        public void Dispose()
        {
            traceContext.Dispose();
        }

        public bool IsActive
        {
            get { return traceContext.IsActive; }
        }

        public string TraceId
        {
            get { return traceContext.TraceId; }
        }

        public string ContextId
        {
            get { return traceContext.ContextId; }
        }

        public string ParentContextId
        {
            get { return traceContext.ParentContextId; }
        }

        public void RecordTimepoint(Timepoint type)
        {
            traceContext.RecordTimepoint(type);
        }

        public void RecordTimepoint(Timepoint type, DateTime time)
        {
            traceContext.RecordTimepoint(type, time);
        }

        public void RecordAnnotation(Annotation type, string value)
        {
            traceContext.RecordAnnotation(type, value);
        }

        public void RecordAnnotation(string name, string value)
        {
            traceContext.RecordAnnotation(name, value);
        }

        private readonly ITraceContext traceContext;
    }
}