using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kontur.Tracing.Core.TraceCases
{
    public static class CommonTraceCases
    {
        public static void RootCase()
        {
            using (var rootContext = Trace.CreateRootContext("Root"))
            {
                rootContext.RecordTimepoint(Timepoint.Start);
                rootContext.RecordTimepoint(Timepoint.Finish);
            }
        }

        public static void RootChildCase()
        {
            using (var rootContext = Trace.CreateRootContext("Root"))
            {
                rootContext.RecordTimepoint(Timepoint.Start);
                using (var childContext = Trace.CreateChildContext("Child"))
                {
                    childContext.RecordTimepoint(Timepoint.Start);
                    childContext.RecordTimepoint(Timepoint.Finish);
                }
                rootContext.RecordTimepoint(Timepoint.Finish);
            }
        }

        public static void RootChildContinueCase()
        {
            string traceId = null;
            string rootContextId = null;

            using (var rootContext = Trace.CreateRootContext("Root"))
            {
                rootContext.RecordTimepoint(Timepoint.Start);
                traceId = rootContext.TraceId;
                rootContextId = rootContext.ContextId;

                using (var childContext = Trace.CreateChildContext("Child"))
                {
                    childContext.RecordTimepoint(Timepoint.Start);
                    childContext.RecordTimepoint(Timepoint.Finish);
                }
            }

            using (var rootContext = Trace.ContinueContext(traceId, rootContextId, isActive: true, isRoot: false))
            {
                rootContext.RecordTimepoint(Timepoint.Finish);
            }
        }

        public static void RootChildContinueFinishCase()
        {
            string traceId = null;
            string childContextId = null;

            using (var rootContext = Trace.CreateRootContext("Root"))
            {
                rootContext.RecordTimepoint(Timepoint.Start);
                using (var childContext = Trace.CreateChildContext("Child"))
                {
                    childContext.RecordTimepoint(Timepoint.Start);
                    childContext.RecordTimepoint(Timepoint.Finish);
                }

                using (var childContext = Trace.CreateChildContext("ChildWithContinue"))
                {
                    childContext.RecordTimepoint(Timepoint.Start);
                    traceId = childContext.TraceId;
                    childContextId = childContext.ContextId;
                }

                var disposeChildContext = Trace.CreateChildContext("ChildWithFinish");
                disposeChildContext.RecordTimepoint(Timepoint.Start);
                disposeChildContext.RecordTimepoint(Timepoint.Finish);
                Trace.FinishCurrentContext();
                rootContext.RecordTimepoint(Timepoint.Finish);
            }
            using (var childContext = Trace.ContinueContext(traceId, childContextId, isActive: true, isRoot: false))
            {
                childContext.RecordTimepoint(Timepoint.Finish);
            }
        }

        public static void ClientServerCase()
        {
            const string url = "http://localhost:12346/";
            var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            var serverTask = Task.Factory.StartNew(() =>
            {
                var context = listener.GetContext();
                var request = context.Request;
                string traceId, contextId;
                bool? isActive;
                RequestExtensions.ExtractFromHttpHeaders(request.Headers, out traceId, out contextId, out isActive);
                using (var serverContext = Trace.ContinueContext(traceId, contextId, isActive ?? false, isRoot: false))
                {
                    serverContext.RecordTimepoint(Timepoint.ServerReceive);
                    context.Response.Close();
                    serverContext.RecordTimepoint(Timepoint.ServerSend);
                }
            });

            using (var clientContext = Trace.CreateRootContext("Client"))
            {
                clientContext.RecordTimepoint(Timepoint.ClientSend);
                clientContext.RecordAnnotation(Annotation.RequestUrl, url);
                var clientRequest = (HttpWebRequest)WebRequest.Create(url);
                clientRequest.SetTracingHeaders(clientContext);
                clientRequest.GetResponse();
                serverTask.Wait();
                clientContext.RecordTimepoint(Timepoint.ClientReceive);
            }

            listener.Close();
        }
    }
}