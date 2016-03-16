using System;
using System.Threading;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Logging;

namespace Kontur.Tracing.Core.Impl
{
    internal class TraceInfoSenderDaemon
    {
        public TraceInfoSenderDaemon(IConfigurationProvider configProvider, ITraceInfoStorage storage, ITraceInfoSender sender)
        {
            this.configProvider = configProvider;
            this.storage = storage;
            this.sender = sender;
            senderThread = new Thread(SendRoutine)
            {
                IsBackground = true,
                Name = "dtrace-sender",
            };
        }

        public void Start()
        {
            lock (locker)
            {
                stopSignal.Reset();
                senderThread.Start();
            }
        }

        public void Stop()
        {
            var tracingConfig = configProvider.GetConfig();
            lock (locker)
            {
                stopSignal.Set();
                senderThread.Join((int)(tracingConfig.BufferFlushTimeout + tracingConfig.BufferFlushPeriod).TotalMilliseconds);
            }
        }

        private void SendRoutine()
        {
            try
            {
                do
                {
                    SendIfNeeded();
                } while (!stopSignal.Wait(configProvider.GetConfig().BufferFlushPeriod));
                SendIfNeeded();
            }
            catch (Exception e)
            {
                log.ErrorException("Unhandled exception on TraceInfoSenderDaemon thread", e);
            }
        }

        private void SendIfNeeded()
        {
            if (!configProvider.GetConfig().IsEnabled)
                return;

            var infos = storage.GetAll();
            if (infos.Count == 0)
                return;

            var sendResult = sender.Send(infos);
            if (sendResult != TraceSpanSendResult.Failure)
                storage.Remove(infos);
        }

        private readonly IConfigurationProvider configProvider;
        private readonly ITraceInfoStorage storage;
        private readonly ITraceInfoSender sender;

        private readonly object locker = new object();
        private readonly Thread senderThread;
        private readonly ManualResetEventSlim stopSignal = new ManualResetEventSlim();

        private static readonly ILog log = LogProvider.GetLogger(typeof(TraceInfoSenderDaemon));
    }
}