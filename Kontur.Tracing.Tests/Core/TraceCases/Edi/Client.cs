using System;
using System.Net;
using System.Text;

namespace Kontur.Tracing.Core.TraceCases.Edi
{
    internal class Client
    {
        public Client(string url, Synchronizer synchronizer)
        {
            this.url = url;
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

                    var bytes = Encoding.ASCII.GetBytes(message);

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;
                    request.Timeout = (int)TimeSpan.FromSeconds(2).TotalMilliseconds;
                    request.SetTracingHeaders(clientContext);
                    var stream = request.GetRequestStream();
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    var response = request.GetResponse();

                    clientContext.RecordTimepoint(Timepoint.ClientReceive);
                    synchronizer.ServerEndProcessQuerySignal.Wait();
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Unhandled client exception: {0}", e);
            }
            finally
            {
                synchronizer.ClientEndQuerySignal.Set();
            }
        }

        private readonly string url;
        private readonly Synchronizer synchronizer;
    }
}