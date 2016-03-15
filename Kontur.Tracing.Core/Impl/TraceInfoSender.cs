using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Text;
using JetBrains.Annotations;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Logging;

namespace Kontur.Tracing.Core.Impl
{
    internal class TraceInfoSender : ITraceInfoSender
    {
        public TraceInfoSender(IConfigurationProvider configProvider, TraceInfoSerializer serializer)
        {
            this.configProvider = configProvider;
            this.serializer = serializer;
        }

        public TraceSpanSendResult Send([NotNull] IList<TraceContextInfo> infos)
        {
            try
            {
                var content = Encoding.UTF8.GetBytes(serializer.Serialize(infos));
                var request = PrepareWebRequest(content);
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(content, 0, content.Length);
                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                return TraceSpanSendResult.Success;
                            case HttpStatusCode.BadRequest:
                                LogFailure(request.RequestUri.Authority, response.StatusCode);
                                return TraceSpanSendResult.IncorrectRequest;
                            default:
                                LogFailure(request.RequestUri.Authority, response.StatusCode);
                                return TraceSpanSendResult.Failure;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogFailure(e);
                return TraceSpanSendResult.Failure;
            }
        }

        private HttpWebRequest PrepareWebRequest(byte[] content)
        {
            var config = configProvider.GetConfig();
            var webRequestUrl = string.Format("{0}?system={1}", config.AggregationServiceUrl, config.AggregationServiceSystem);
            var webRequest = WebRequest.CreateHttp(webRequestUrl);

            webRequest.Method = "POST";
            webRequest.ContentLength = content.Length;
            webRequest.ContentType = "application/x-ldjson";

            webRequest.Expect = null;
            webRequest.Proxy = null;
            webRequest.KeepAlive = true;
            webRequest.Pipelined = true;

            webRequest.AllowWriteStreamBuffering = false;
            webRequest.AllowReadStreamBuffering = false;
            webRequest.AuthenticationLevel = AuthenticationLevel.None;
            webRequest.AutomaticDecompression = DecompressionMethods.None;

            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.ServicePoint.ConnectionLimit = 100;
            webRequest.ServicePoint.UseNagleAlgorithm = false;

            var timeoutMilliseconds = Math.Max(1, (int) config.BufferFlushTimeout.TotalMilliseconds);
            webRequest.Timeout = timeoutMilliseconds;
            webRequest.ReadWriteTimeout = timeoutMilliseconds;

            return webRequest;
        }

        private static void LogFailure(Exception e)
        {
            log.ErrorException("Failure in sending spans", e);
        }

        private static void LogFailure(string address, HttpStatusCode code)
        {
            log.Error(string.Format("Failure in sending spans: received unexpected response code '{0}' from: {1}", code, address));
        }

        private readonly IConfigurationProvider configProvider;
        private readonly TraceInfoSerializer serializer;

        private static readonly ILog log = LogProvider.GetCurrentClassLogger();
    }
}