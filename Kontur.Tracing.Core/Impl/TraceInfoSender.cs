using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
                using (var httpClient = new HttpClient(){
                    
                })
                {
                    var config = configProvider.GetConfig();
                    var url = string.Format("{0}?system={1}", config.AggregationServiceUrl, config.AggregationServiceSystem);
                    using (var content = new ByteArrayContent(Encoding.UTF8.GetBytes(serializer.Serialize(infos))))
                    {
                        var response = httpClient.PostAsync(url, content).Result;
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                return TraceSpanSendResult.Success;
                            case HttpStatusCode.BadRequest:
                                LogFailure(url, response.StatusCode);
                                return TraceSpanSendResult.IncorrectRequest;
                            default:
                                LogFailure(url, response.StatusCode);
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

        private static readonly ILog log = LogProvider.GetLogger(typeof(TraceInfoSender));
    }
}