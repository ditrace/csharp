using System.Collections.Generic;

namespace Kontur.Tracing.Core
{
    public class TraceContextDescriptor
    {
        public string TraceId { get; set; }
        public string ContextId { get; set; }
        public string ParentContextId { get; set; }
        public string ContextName { get; set; }
        public bool IsRoot { get; set; }
        public Dictionary<Annotation, string> Annotations { get; set; }
        public Dictionary<Timepoint, int> Timeline { get; set; }
    }
}