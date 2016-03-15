using System;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal class NoOpTraceContext : ITraceContext
    {
        public static readonly NoOpTraceContext Instance = new NoOpTraceContext();

        public bool IsActive
        {
            get { return false; }
        }

        [NotNull]
        public string TraceId
        {
            get { return string.Empty; }
        }

        [NotNull]
        public string ContextId
        {
            get { return string.Empty; }
        }

        [CanBeNull]
        public string ParentContextId
        {
            get { return null; }
        }

        public void RecordTimepoint(Timepoint type)
        {
        }

        public void RecordTimepoint(Timepoint type, DateTime time)
        {
        }

        public void RecordAnnotation(Annotation type, [NotNull] string value)
        {
        }

        public void RecordAnnotation([NotNull] string name, [NotNull] string value)
        {
        }

        public void Dispose()
        {
        }

        public void Dispose(bool flush)
        {
        }
    }
}