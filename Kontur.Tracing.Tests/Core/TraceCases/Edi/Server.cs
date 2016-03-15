using System;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class Server : IDisposable
    {
        public Server(string url, RemoteTaskQueue taskQueue, Synchronizer synchronizer)
        {
            this.taskQueue = taskQueue;
            this.synchronizer = synchronizer;
            listener.Prefixes.Add(url);
            listener.Start();
            processTask = Task.Factory.StartNew(() => { HandleContext(listener.GetContext()); });
        }

        public void Dispose()
        {
            processTask.Wait();
            listener.Close();
        }

        private void HandleContext(HttpListenerContext context)
        {
            try
            {
                string traceId, contextId;
                bool? isActive;
                RequestExtensions.ExtractFromHttpHeaders(context.Request.Headers, out traceId, out contextId, out isActive);
                using (var serverContext = Trace.ContinueContext(traceId, contextId, isActive ?? false, isRoot: false))
                {
                    serverContext.RecordTimepoint(Timepoint.ServerReceive);
                    serverContext.RecordAnnotation(Annotation.RequestUrl, context.Request.Url.ToString());

                    using(var streamReader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    {
                        var message = streamReader.ReadToEnd();
                        taskQueue.PushTask(new RemoteTask(message));
                        context.Response.StatusCode = 202;
                        context.Response.Close();
                    }

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
        private readonly Task processTask;
        private readonly HttpListener listener = new HttpListener();
    }
}