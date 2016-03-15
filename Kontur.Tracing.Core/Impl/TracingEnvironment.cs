using JetBrains.Annotations;
using Kontur.Tracing.Core.Config;

namespace Kontur.Tracing.Core.Impl
{
    internal class TracingEnvironment : ITracingEnvironment
    {
        [NotNull]
        public ITimeProvider TimeProvider { get; private set; }

        [NotNull]
        public IAnnotationNameMapper AnnotationNameMapper { get; private set; }

        [NotNull]
        public ITraceSampler TraceSampler { get; private set; }

        [NotNull]
        public ITraceInfoStorage TraceInfoStorage { get; private set; }

        public bool IsStarted
        {
            get { return isStarted; }
        }

        public void Start([NotNull] IConfigurationProvider configProvider)
        {
            TimeProvider = new TimeProvider();
            AnnotationNameMapper = new AnnotationNameMapper();
            TraceSampler = new TraceSampler(configProvider, TimeProvider);
            TraceInfoStorage = new TraceInfoStorage(configProvider);
            var sender = new TraceInfoSender(configProvider, new TraceInfoSerializer());
            daemon = new TraceInfoSenderDaemon(configProvider, TraceInfoStorage, sender);
            daemon.Start();
            isStarted = true;
        }

        public void Stop()
        {
            isStarted = false;
            daemon.Stop();
        }

        private volatile bool isStarted;
        private TraceInfoSenderDaemon daemon;
    }
}