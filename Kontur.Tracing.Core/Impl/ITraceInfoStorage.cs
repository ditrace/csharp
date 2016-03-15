using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal interface ITraceInfoStorage
    {
        bool TryAdd([NotNull] TraceContextInfo info);

        void Remove([NotNull] IEnumerable<TraceContextInfo> toRemove);

        [NotNull]
        IList<TraceContextInfo> GetAll();
    }
}