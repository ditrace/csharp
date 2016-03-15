using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Kontur.Tracing.Core.Config;

namespace Kontur.Tracing.Core.Impl
{
    internal class TraceInfoStorage : ITraceInfoStorage
    {
        public TraceInfoStorage(IConfigurationProvider configProvider)
        {
            this.configProvider = configProvider;
            infos = new HashSet<TraceContextInfo>();
            syncObject = new object();
        }

        public bool TryAdd([NotNull] TraceContextInfo info)
        {
            var capacityLimit = configProvider.GetConfig().MaxBufferedSpans;
            lock (syncObject)
            {
                if (infos.Count >= capacityLimit)
                    return false;
                infos.Add(info);
                return true;
            }
        }

        public void Remove([NotNull] IEnumerable<TraceContextInfo> toRemove)
        {
            lock (syncObject)
            {
                foreach (var info in toRemove)
                    infos.Remove(info);
            }
        }

        [NotNull]
        public IList<TraceContextInfo> GetAll()
        {
            lock (syncObject)
                return infos.ToList();
        }

        private readonly IConfigurationProvider configProvider;
        private readonly HashSet<TraceContextInfo> infos;
        private readonly object syncObject;
    }
}