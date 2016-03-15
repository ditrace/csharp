using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal interface IAnnotationNameMapper
    {
        [NotNull]
        string Map(Annotation annotation);

        [NotNull]
        string Map(Timepoint timepoint);
    }
}