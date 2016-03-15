using System;
using System.Collections.Generic;
using Kontur.Tracing.Core.Config;

namespace Kontur.Tracing.Core.Impl
{
    internal class TraceSampler : ITraceSampler
    {
        public TraceSampler(IConfigurationProvider configProvider, ITimeProvider timeProvider)
        {
            this.configProvider = configProvider;
            this.timeProvider = timeProvider;
            random = new Random();
            lastSamples = new Queue<DateTime>();
        }

        public bool CanSampleTrace()
        {
            var config = configProvider.GetConfig();
            lock (random)
            {
                if (random.NextDouble() > config.SamplingChance)
                    return false;
            }

            var currentTime = timeProvider.GetCurrentTime();
            lock (lastSamples)
            {
                while (lastSamples.Count > 0 && (currentTime - lastSamples.Peek()).TotalSeconds >= 1)
                    lastSamples.Dequeue();

                if (lastSamples.Count >= config.MaxSamplesPerSecond)
                    return false;

                lastSamples.Enqueue(currentTime);
            }

            return true;
        }

        private readonly IConfigurationProvider configProvider;
        private readonly ITimeProvider timeProvider;
        private readonly Random random;
        private readonly Queue<DateTime> lastSamples;
    }
}