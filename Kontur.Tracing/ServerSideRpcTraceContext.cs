using JetBrains.Annotations;

namespace Kontur.Tracing
{
    internal class ServerSideRpcTraceContext : RpcTraceContextBase
    {
        public ServerSideRpcTraceContext([NotNull] string serverContextName, [CanBeNull] string traceId, [CanBeNull] string contextId, bool isActive)
            : base(!string.IsNullOrEmpty(traceId) && !string.IsNullOrEmpty(contextId)
                ? Core.Trace.ContinueContext(traceId, contextId, isActive, isRoot: false)
                : Core.Trace.CreateRootContext(serverContextName))
        {
        }
    }
}