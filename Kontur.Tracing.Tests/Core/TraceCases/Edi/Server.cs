using System;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class Server
    {
        public Server(RemoteTaskQueue taskQueue, Synchronizer synchronizer)
        {
            this.taskQueue = taskQueue;
            this.synchronizer = synchronizer;
        }

        public void HandleContext(ClientRequest request)
        {
            try
            {
                string traceId, contextId;
                bool? isActive;
                RequestExtensions.ExtractFromHttpHeaders(request.Raw.Headers, out traceId, out contextId, out isActive);
                using (var serverContext = Trace.ContinueContext(traceId, contextId, isActive ?? false, isRoot: false))
                {
                    serverContext.RecordTimepoint(Timepoint.ServerReceive);
                    serverContext.RecordAnnotation(Annotation.RequestUrl, request.Raw.RequestUri.ToString());
                    taskQueue.PushTask(new RemoteTask(request.Message));
                    serverContext.RecordTimepoint(Timepoint.ServerSend);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Unhandled server exception: {0}", e);
            }
            finally
            {
                synchronizer.ServerEndProcessQuerySignal.Set();
            }
        }

        private readonly RemoteTaskQueue taskQueue;
        private readonly Synchronizer synchronizer;
    }
}