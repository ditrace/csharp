using System;
using System.Net;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class Client
    {
        private readonly string url = "/url";
        private Server server;
        private readonly Synchronizer synchronizer;

        public Client(Synchronizer synchronizer, Server server)
        {
            this.server = server;
            this.synchronizer = synchronizer;
        }

        public void SendMessage(string message)
        {
            try
            {
                using (var clientContext = Trace.CreateRootContext("Client-Server"))
                {
                    Console.WriteLine(clientContext.TraceId);
                    clientContext.RecordAnnotation(Annotation.RequestUrl, url);
                    clientContext.RecordTimepoint(Timepoint.ClientSend);

                    var request = new ClientRequest((HttpWebRequest)WebRequest.Create(url), message);
                    request.Raw.SetTracingHeaders(clientContext);
                    
                    server.HandleContext(request);

                    clientContext.RecordTimepoint(Timepoint.ClientReceive);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Unhandled client exception: {0}", e);
            }
        }
    }
}