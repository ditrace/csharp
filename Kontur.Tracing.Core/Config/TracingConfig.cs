using System;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Config
{
    public class TracingConfig : ITracingConfig
    {
        public TracingConfig(bool isEnabled, [NotNull] string aggregationServiceSystem, [NotNull] string aggregationServiceUrl)
        {
            IsEnabled = isEnabled;
            AggregationServiceSystem = aggregationServiceSystem;
            AggregationServiceUrl = aggregationServiceUrl;
            MaxBufferedSpans = 10000;
            BufferFlushPeriod = TimeSpan.FromSeconds(5);
            BufferFlushTimeout = TimeSpan.FromSeconds(30);
            SamplingChance = 0.01;
            MaxSamplesPerSecond = 20;
        }

        public bool IsEnabled { get; private set; }

        [NotNull]
        public string AggregationServiceSystem { get; private set; }

        [NotNull]
        public string AggregationServiceUrl { get; private set; }

        public int MaxBufferedSpans { get; set; }

        public TimeSpan BufferFlushPeriod { get; set; }

        public TimeSpan BufferFlushTimeout { get; set; }

        public double SamplingChance { get; set; }

        public int MaxSamplesPerSecond { get; set; }
    }
}