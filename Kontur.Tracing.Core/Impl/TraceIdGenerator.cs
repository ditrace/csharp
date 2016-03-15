using System;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    public static class TraceIdGenerator
    {
        [NotNull]
        public static string CreateTraceId()
        {
            return NewGuid();
        }

        [NotNull]
        public static string CreateTraceContextId()
        {
            return NewGuid().Substring(0, 8);
        }

        [NotNull]
        private static string NewGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}