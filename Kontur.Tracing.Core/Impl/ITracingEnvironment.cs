using JetBrains.Annotations;
using Kontur.Tracing.Core.Config;

namespace Kontur.Tracing.Core.Impl
{
    internal interface ITracingEnvironment
    {
        [NotNull]
        ITimeProvider TimeProvider { get; }

        [NotNull]
        IAnnotationNameMapper AnnotationNameMapper { get; }

        [NotNull]
        ITraceSampler TraceSampler { get; }

        [NotNull]
        ITraceInfoStorage TraceInfoStorage { get; }

        bool IsStarted { get; }

        void Start([NotNull] IConfigurationProvider configProvider);

        void Stop();
    }
}