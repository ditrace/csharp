using System;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class RemoteTask
    {
        public string TraceId { get; set; }
        public string TaskId { get; private set; }
        public string QueueContextId { get; set; }
        public string Message { get; private set; }

        public RemoteTask(string message)
        {
            TaskId = Guid.NewGuid().ToString();
            Message = message;
        }
    }
}