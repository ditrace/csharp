using System.Collections.Generic;
using Kontur.Tracing.Core.Impl;

namespace Kontur.Tracing.Core
{
    internal class FakeTraceInfoStorage : ITraceInfoStorage
    {
        public bool TryAdd(TraceContextInfo info)
        {
            lock (syncObject)
            {
                infos.Add(info);
                return true;
            }
        }

        public void Remove(IEnumerable<TraceContextInfo> toRemove)
        {
        }

        public IList<TraceContextInfo> GetAll()
        {
            lock (syncObject)
                return infos;
        }

        private readonly List<TraceContextInfo> infos = new List<TraceContextInfo>();
        private readonly object syncObject = new object();
    }
}
