namespace Kontur.Tracing.Core.Impl
{
    internal interface ITraceSampler
    {
        bool CanSampleTrace();
    }
}