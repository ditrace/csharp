using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class HandlerManager : IDisposable
    {
        public HandlerManager(RemoteTaskQueue queue, Synchronizer synchronizer)
        {
            taskQueue = queue;
            this.synchronizer = synchronizer;
            processTask = Task.Factory.StartNew(Process);
        }

        public void Dispose()
        {
            stopSignal.Set();
            processTask.Wait();
        }

        private void Process()
        {
            synchronizer.ClientEndQuerySignal.Wait();
            do
            {
                var task = taskQueue.PopTask();
                if (task != null)
                {
                    using (var taskContext = Trace.ContinueContext(task.TraceId, task.TaskId, isActive: true, isRoot: false))
                    {
                        using (var handleContext = Trace.CreateChildContext("Handling"))
                        {
                            handleContext.RecordTimepoint(Timepoint.Start);
                            handler.Handle(task);
                            handleContext.RecordTimepoint(Timepoint.Finish);
                        }
                        taskContext.RecordTimepoint(Timepoint.Finish);
                    }
                }
            } while (!stopSignal.Wait(TimeSpan.FromSeconds(1)));
        }

        private readonly RemoteTaskQueue taskQueue;
        private readonly Synchronizer synchronizer;
        private readonly Task processTask;
        private readonly TaskHandler handler = new TaskHandler();
        private readonly ManualResetEventSlim stopSignal = new ManualResetEventSlim();
    }
}