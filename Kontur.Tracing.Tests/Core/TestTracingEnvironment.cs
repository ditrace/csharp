using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Impl;

namespace Kontur.Tracing.Core
{
    internal class TestTracingEnvironment : ITracingEnvironment
    {
        public ITimeProvider TimeProvider { get; private set; }
        public IAnnotationNameMapper AnnotationNameMapper { get; private set; }
        public ITraceSampler TraceSampler { get; private set; }
        public ITraceInfoStorage TraceInfoStorage { get; private set; }

        public bool IsStarted
        {
            get { return true; }
        }

        public void Start(IConfigurationProvider configProvider)
        {
            TimeProvider = new TimeProvider();
            AnnotationNameMapper = new AnnotationNameMapper();
            TraceSampler = new TraceSampler(configProvider, TimeProvider);
            TraceInfoStorage = new FakeTraceInfoStorage();
        }

        public void Stop()
        {
        }
    }
}