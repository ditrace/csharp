using System.Net;

namespace Kontur.Tracing.Core
{
    internal class ClientRequest
    {
        public HttpWebRequest Raw {get; set;}
        
        public string Message {get; set;}

        public ClientRequest(HttpWebRequest raw, string message)
        {
            Raw = raw;
            Message = message;
        }
    }
}