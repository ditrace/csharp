using System;

namespace Kontur.Tracing.Core.Impl
{
    internal interface ITimeProvider
    {
        DateTime GetCurrentTime();
    }
}