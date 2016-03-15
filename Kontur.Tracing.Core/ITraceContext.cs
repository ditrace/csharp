using System;

namespace Kontur.Tracing.Core
{
    public interface ITraceContext : ITraceContextAnnotator, IDisposable
    {
        void Dispose(bool flush);
    }
}