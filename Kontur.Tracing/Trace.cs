using System;
using JetBrains.Annotations;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Logging;

namespace Kontur.Tracing
{
    public static class Trace
    {
        [NotNull]
        public static IRpcTraceContext BeginClientSpan(ILog log = null)
        {
            InitializeIfNeeded(log);
            return new ClientSideRpcTraceContext("target-id-to-be-overwritten-via-set-annotation");
        }

        [NotNull]
        public static IRpcTraceContext BeginServerSpan([CanBeNull] string traceId, [CanBeNull] string contextId, bool isActive, ILog log = null)
        {
            InitializeIfNeeded(log);
            return new ServerSideRpcTraceContext("target-id-to-be-overwritten-via-set-annotation", traceId, contextId, isActive);
        }

        public static void Initialize([CanBeNull] IConfigurationProvider configurationProvider, ILog log = null)
        {
            lock (initializationSync)
            {
                if (isInitialized)
                    throw new InvalidOperationException(string.Format("Tracing system is already initialized with configuration provider of type '{0}'!", configProvider.GetType().FullName));
                configProvider = configurationProvider;
                InitializeIfNeeded(log);
            }
        }

        public static void Stop()
        {
            lock (initializationSync)
            {
                if (!isInitialized)
                    return;
                Core.Trace.Stop();
                isInitialized = false;
            }
        }

        public static bool IsInitialized
        {
            get { return isInitialized; }
        }

        private static void InitializeIfNeeded(ILog log)
        {
            if (isInitialized)
                return;

            lock (initializationSync)
            {
                if (isInitialized)
                    return;

                Core.Trace.Initialize(configProvider);

                isInitialized = true;
            }
        }

        private static IConfigurationProvider configProvider;

        private static readonly object initializationSync = new object();
        private static volatile bool isInitialized;
    }
}