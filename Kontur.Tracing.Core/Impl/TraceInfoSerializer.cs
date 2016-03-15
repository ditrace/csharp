using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal class TraceInfoSerializer
    {
        public TraceInfoSerializer()
        {
            localBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder(8192));
        }

        [NotNull]
        public string Serialize([NotNull] IEnumerable<TraceContextInfo> infos)
        {
            var builder = localBuilder.Value.Clear();
            foreach (var info in infos.Select(ToSpanDto))
            {
                builder
                    .Append('{')
                    .AppendKeyValuePair("traceId", info.TraceId).AppendComma()
                    .AppendKeyValuePair("spanId", info.SpanId).AppendComma();
                if (!string.IsNullOrEmpty(info.ParentSpanId))
                    builder.AppendKeyValuePair("parentSpanId", info.ParentSpanId).AppendComma();
                var timeline = info.Timeline.Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.ToString("O")));
                builder
                    .AppendPlainObject("annotations", info.Annotations)
                    .AppendComma()
                    .AppendPlainObject("timeline", timeline)
                    .Append('}')
                    .Append(Environment.NewLine);
            }
            if (builder.Length > 0)
                builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return builder.ToString();
        }

        [NotNull]
        private static SpanDto ToSpanDto([NotNull] TraceContextInfo traceContextInfo)
        {
            var annotations = traceContextInfo.Annotations.ToDictionary(x => x.Key, x => x.Value);
            if (!string.IsNullOrEmpty(traceContextInfo.ContextName) && !traceContextInfo.Annotations.ContainsKey("targetId"))
                annotations["targetId"] = traceContextInfo.ContextName;
            if (traceContextInfo.IsRoot)
                annotations["root"] = "true";
            return new SpanDto
            {
                TraceId = traceContextInfo.TraceId,
                SpanId = traceContextInfo.ContextId,
                ParentSpanId = traceContextInfo.ParentContextId,
                Timeline = traceContextInfo.Timeline.ToDictionary(x => x.Key, x => x.Value),
                Annotations = annotations,
            };
        }

        private readonly ThreadLocal<StringBuilder> localBuilder;
    }
}