using System;
using JetBrains.Annotations;
using Kontur.Tracing.Core.Logging;

namespace Kontur.Tracing.Core.Impl
{
    internal class RealTraceContext : MarshalByRefObject, ITraceContext
    {
        public RealTraceContext([NotNull] string traceId, [NotNull] string contextId, [CanBeNull] string contextName, [CanBeNull] string parentContextId, [NotNull] ITracingEnvironment tracingEnvironment, bool isRoot)
        {
            this.tracingEnvironment = tracingEnvironment;
            traceContextInfo = new TraceContextInfo(isRoot, traceId, contextId, contextName, parentContextId);
            previousRealTraceContext = TraceContext.Current as RealTraceContext;
        }

        public bool IsActive
        {
            get { return true; }
        }

        [NotNull]
        public string TraceId
        {
            get { return traceContextInfo.TraceId; }
        }

        [NotNull]
        public string ContextId
        {
            get { return traceContextInfo.ContextId; }
        }

        [CanBeNull]
        public string ParentContextId
        {
            get { return traceContextInfo.ParentContextId; }
        }

        public void RecordTimepoint(Timepoint type)
        {
            RecordTimepoint(type, tracingEnvironment.TimeProvider.GetCurrentTime());
        }

        public void RecordTimepoint(Timepoint type, DateTime time)
        {
            traceContextInfo.SetTimepoint(tracingEnvironment.AnnotationNameMapper.Map(type), time);
        }

        public void RecordAnnotation(Annotation type, [NotNull] string value)
        {
            RecordAnnotation(tracingEnvironment.AnnotationNameMapper.Map(type), value);
        }

        public void RecordAnnotation([NotNull] string name, [NotNull] string value)
        {
            traceContextInfo.SetAnnotation(name, value);
        }

        public void Dispose()
        {
            DoDispose(flush: true);
        }

        public void Dispose(bool flush)
        {
            DoDispose(flush);
        }

        private void DoDispose(bool flush)
        {
            if (isDisposed)
                return;
            DoFinish(flush);
            isDisposed = true;
        }

        internal void Finish()
        {
            DoFinish(flush: true);
        }

        private void DoFinish(bool flush)
        {
            if (flush)
                Flush();
            Trace.PopRealTraceContext(previousRealTraceContext);
        }

        private void Flush()
        {
            if (!tracingEnvironment.IsStarted)
                log.Warn(string.Format("Trace is likely to be stopped. Context will not be flushed: {0}", this));
            else
            {
                if (!tracingEnvironment.TraceInfoStorage.TryAdd(traceContextInfo))
                    log.Warn(string.Format("Failed to flush context: {0}", this));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, PreviousRealTraceContext: {1}", traceContextInfo, previousRealTraceContext);
        }

        private bool isDisposed;
        private readonly ITracingEnvironment tracingEnvironment;
        private readonly TraceContextInfo traceContextInfo;
        private readonly RealTraceContext previousRealTraceContext;
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();
    }
}