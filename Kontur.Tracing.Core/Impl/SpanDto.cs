using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal class SpanDto
    {
        [NotNull]
        public string TraceId { get; set; }

        [NotNull]
        public string SpanId { get; set; }

        [CanBeNull]
        public string ParentSpanId { get; set; }

        [NotNull]
        public Dictionary<string, DateTime> Timeline { get; set; }

        [NotNull]
        public Dictionary<string, string> Annotations { get; set; }
    }
}