using System;
using System.Runtime.Remoting.Messaging;
using JetBrains.Annotations;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Impl;

namespace Kontur.Tracing.Core
{
    public static class Trace
    {
        [NotNull]
        public static ITraceContext CreateRootContext([NotNull] string contextName, string traceId = null, bool? activate = null)
        {
            if (string.IsNullOrEmpty(contextName))
                throw new InvalidOperationException("ContextName is empty");

            InitializeIfNeeded();
            if (!configProvider.GetConfig().IsEnabled)
                return NoOpTraceContext.Instance;

            var isSampled = activate ?? tracingEnvironment.TraceSampler.CanSampleTrace();
            if (!isSampled)
                return NoOpTraceContext.Instance;

            if (string.IsNullOrEmpty(traceId))
                traceId = TraceIdGenerator.CreateTraceId();
            var rootContextId = TraceIdGenerator.CreateTraceContextId();
            var rootContext = new RealTraceContext(traceId, rootContextId, contextName, null, tracingEnvironment, isRoot: true);
            SetRealTraceContext(rootContext);
            return rootContext;
        }

        [NotNull]
        public static ITraceContext CreateChildContext([NotNull] string contextName, string contextId = null)
        {
            if (string.IsNullOrEmpty(contextName))
                throw new InvalidOperationException("ContextName is empty");

            var currentContext = TryGetRealTraceContext();
            if (currentContext == null)
                return NoOpTraceContext.Instance;

            InitializeIfNeeded();
            if (!configProvider.GetConfig().IsEnabled)
                return NoOpTraceContext.Instance;

            if (string.IsNullOrEmpty(contextId))
                contextId = TraceIdGenerator.CreateTraceContextId();
            var childContext = new RealTraceContext(currentContext.TraceId, contextId, contextName, currentContext.ContextId, tracingEnvironment, isRoot: false);
            SetRealTraceContext(childContext);
            return childContext;
        }

        public static ITraceContext CreateChildContext([NotNull] string contextName, [NotNull] string traceId, [NotNull] string parentContextId, bool? activate = null)
        {
            if (string.IsNullOrEmpty(contextName))
                throw new InvalidOperationException("ContextName is empty");
            if (string.IsNullOrEmpty(traceId))
                throw new InvalidOperationException("TraceId is empty");
            if (string.IsNullOrEmpty(parentContextId))
                throw new InvalidOperationException("ParentContextId is empty");

            InitializeIfNeeded();
            if (!configProvider.GetConfig().IsEnabled)
                return NoOpTraceContext.Instance;

            var isSampled = activate ?? tracingEnvironment.TraceSampler.CanSampleTrace();
            if (!isSampled)
                return NoOpTraceContext.Instance;

            var childContextId = TraceIdGenerator.CreateTraceContextId();
            var childContext = new RealTraceContext(traceId, childContextId, contextName, parentContextId, tracingEnvironment, isRoot: false);
            SetRealTraceContext(childContext);
            return childContext;
        }

        [NotNull]
        public static ITraceContext ContinueContext([NotNull] string traceId, [NotNull] string contextId, bool isActive, bool isRoot)
        {
            if (!isActive)
                return NoOpTraceContext.Instance;
            if (string.IsNullOrEmpty(traceId))
                throw new InvalidOperationException("TraceId is empty");
            if (string.IsNullOrEmpty(contextId))
                throw new InvalidOperationException("ContextId is empty");

            InitializeIfNeeded();
            if (!configProvider.GetConfig().IsEnabled)
                return NoOpTraceContext.Instance;

            var currentContext = new RealTraceContext(traceId, contextId, null, null, tracingEnvironment, isRoot);
            SetRealTraceContext(currentContext);
            return currentContext;
        }

        public static void FinishCurrentContext()
        {
            var currentContext = TryGetRealTraceContext();
            if (currentContext == null)
                return;

            InitializeIfNeeded();
            if (!configProvider.GetConfig().IsEnabled)
                return;

            currentContext.Finish();
        }

        internal static void PopRealTraceContext([CanBeNull] RealTraceContext previousContext)
        {
            SetRealTraceContext(previousContext);
        }

        [CanBeNull]
        private static RealTraceContext TryGetRealTraceContext()
        {
            return (RealTraceContext)CallContext.LogicalGetData(realTraceContextStorageKey);
        }

        private static void SetRealTraceContext([CanBeNull] RealTraceContext newCurrentContext)
        {
            if (newCurrentContext == null)
            {
                CallContext.FreeNamedDataSlot(AnnotatorStorageKey);
                CallContext.FreeNamedDataSlot(realTraceContextStorageKey);
                CallContext.FreeNamedDataSlot(dTraceIdPublicStorageKey);
            }
            else
            {
                CallContext.LogicalSetData(AnnotatorStorageKey, newCurrentContext);
                CallContext.LogicalSetData(realTraceContextStorageKey, newCurrentContext);
                CallContext.LogicalSetData(dTraceIdPublicStorageKey, newCurrentContext.TraceId);
            }
        }

        public static bool IsInitialized
        {
            get { return isInitialized; }
        }

        public static void Initialize([NotNull] IConfigurationProvider theConfigProvider)
        {
            Initialize(theConfigProvider, null);
        }

        internal static void Initialize([NotNull] IConfigurationProvider theConfigProvider, [CanBeNull] ITracingEnvironment theTracingEnvironment)
        {
            lock (initializationSync)
            {
                if (isInitialized)
                    throw new InvalidOperationException(string.Format("Tracing system is already initialized with configuration provider of type '{0}'!", configProvider.GetType().FullName));
                DoInitialize(theConfigProvider, theTracingEnvironment);
            }
        }

        private static void InitializeIfNeeded()
        {
            if (isInitialized)
                return;
            lock (initializationSync)
            {
                if (!isInitialized)
                    DoInitialize(null, null);
            }
        }

        private static void DoInitialize([CanBeNull] IConfigurationProvider theConfigProvider, [CanBeNull] ITracingEnvironment theTracingEnvironment)
        {
            configProvider = theConfigProvider ?? StaticConfigurationProvider.Disabled;
            if (configProvider.GetConfig().IsEnabled == false)
                return;

            tracingEnvironment = theTracingEnvironment ?? new TracingEnvironment();
            tracingEnvironment.Start(configProvider);

            isInitialized = true;
        }

        public static void Stop()
        {
            lock (initializationSync)
            {
                if (!isInitialized)
                    return;

                tracingEnvironment.Stop();
                tracingEnvironment = null;
                configProvider = null;

                isInitialized = false;
            }
        }

        public static readonly string AnnotatorStorageKey = Guid.NewGuid().ToString();
        private static readonly string realTraceContextStorageKey = Guid.NewGuid().ToString();
        // dTraceIdPublicStorageKey is shared with log4stash library
        private const string dTraceIdPublicStorageKey = "DTraceId-PublicStorageKey-33115BA6-3CC9-4BC5-A540-D2EA133B0B7F";

        private static IConfigurationProvider configProvider;
        private static ITracingEnvironment tracingEnvironment;

        private static readonly object initializationSync = new object();
        private static volatile bool isInitialized;
    }
}