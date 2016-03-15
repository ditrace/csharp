using JetBrains.Annotations;

namespace Kontur.Tracing
{
    internal class ClientSideRpcTraceContext : RpcTraceContextBase
    {
        public ClientSideRpcTraceContext([NotNull] string clientContextName)
            : base(Core.Trace.CreateChildContext(clientContextName))
        {
        }
    }
}