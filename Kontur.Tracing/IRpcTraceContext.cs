using System;
using Kontur.Tracing.Core;

namespace Kontur.Tracing
{
    public interface IRpcTraceContext : ITraceContextAnnotator, IDisposable
    {
    }
}