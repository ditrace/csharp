using System.Threading;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class Synchronizer
    {
        public readonly ManualResetEventSlim ServerEndProcessQuerySignal = new ManualResetEventSlim();
        public readonly ManualResetEventSlim ClientEndQuerySignal = new ManualResetEventSlim();
    }
}