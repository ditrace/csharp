using System;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core
{
    public interface ITraceContextAnnotator
    {
        bool IsActive { get; }

        [NotNull]
        string TraceId { get; }

        [NotNull]
        string ContextId { get; }

        [CanBeNull]
        string ParentContextId { get; }

        void RecordTimepoint(Timepoint type);
        void RecordTimepoint(Timepoint type, DateTime time);

        void RecordAnnotation(Annotation type, [NotNull] string value);
        void RecordAnnotation([NotNull] string name, [NotNull] string value);
    }
}