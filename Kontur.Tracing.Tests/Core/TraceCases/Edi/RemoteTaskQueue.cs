using System.Collections.Generic;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class RemoteTaskQueue
    {
        public void PushTask(RemoteTask task)
        {
            using (var taskContext = Trace.CreateChildContext("TaskLife", task.TaskId))
            {
                taskContext.RecordTimepoint(Timepoint.Start);
                task.TraceId = taskContext.TraceId;

                using (var queueContext = Trace.CreateChildContext("Queue"))
                {
                    task.QueueContextId = queueContext.ContextId;

                    queueContext.RecordTimepoint(Timepoint.Start);
                    taskQueue.Enqueue(task);
                }
            }
        }

        public RemoteTask PopTask()
        {
            var task = taskQueue.Count > 0 ? taskQueue.Dequeue() : null;
            if (task != null)
                using (var queueContext = Trace.ContinueContext(task.TraceId, task.QueueContextId, isActive: true, isRoot: false))
                {
                    queueContext.RecordTimepoint(Timepoint.Finish);
                }
            return task;
        }

        private readonly Queue<RemoteTask> taskQueue = new Queue<RemoteTask>();
    }
}