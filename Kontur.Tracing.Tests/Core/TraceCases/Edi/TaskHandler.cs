using System;
using System.Threading;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class TaskHandler
    {
        public void Handle(RemoteTask task)
        {
            Console.WriteLine(task.Message);
            Thread.Sleep(200);
        }
    }
}