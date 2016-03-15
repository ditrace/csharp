using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal interface ITraceInfoSender
    {
        TraceSpanSendResult Send([NotNull] IList<TraceContextInfo> infos);
    }
}