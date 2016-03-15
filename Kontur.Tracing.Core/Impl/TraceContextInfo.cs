using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal class TraceContextInfo
    {
        public TraceContextInfo(bool isRoot, [NotNull] string traceId, [NotNull] string contextId, [CanBeNull] string contextName, [CanBeNull] string parentContextId)
        {
            IsRoot = isRoot;
            TraceId = traceId;
            ContextId = contextId;
            ContextName = contextName;
            ParentContextId = parentContextId;
            Timeline = new Dictionary<string, DateTime>();
            Annotations = new Dictionary<string, string>();
        }

        public bool IsRoot { get; private set; }

        [NotNull]
        public string TraceId { get; private set; }

        [NotNull]
        public string ContextId { get; private set; }

        [CanBeNull]
        public string ParentContextId { get; private set; }

        [CanBeNull]
        public string ContextName { get; private set; }

        [NotNull]
        public Dictionary<string, DateTime> Timeline { get; internal set; }

        [NotNull]
        public Dictionary<string, string> Annotations { get; internal set; }

        public void SetTimepoint([NotNull] string type, DateTime value)
        {
            if (!string.IsNullOrEmpty(type))
                Timeline[type] = value;
        }

        public void SetAnnotation([NotNull] string name, [NotNull] string value)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                Annotations[name] = value;
        }

        public override string ToString()
        {
            return string.Format("TraceId: {0}, ContextId: {1}, IsRoot: {2}, ContextName: {3}, ParentContextId: {4}", TraceId, ContextId, IsRoot, ContextName, ParentContextId);
        }
    }
}