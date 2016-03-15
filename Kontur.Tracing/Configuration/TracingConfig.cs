using System;
using Kontur.Tracing.Core.Config;

namespace Kontur.Tracing
{
    public class TracingConfig : ITracingConfig
    {
        public bool IsEnabled = false;

        public string AggregationServiceURL = "http://localhost:9003/spans";

        public string AggregationServiceSystem = "ke";

        public int MaxBufferedSpans = 10000;

        public TimeSpan BufferFlushPeriod = TimeSpan.FromSeconds(5);

        public TimeSpan BufferFlushTimeout = TimeSpan.FromSeconds(30);

        public double SamplingChance = 0.01D;

        public int MaxSamplesPerSecond = 20;

        bool ITracingConfig.IsEnabled
        {
            get { return IsEnabled; }
        }

        string ITracingConfig.AggregationServiceSystem
        {
            get { return AggregationServiceSystem; }
        }

        public string AggregationServiceUrl
        {
            get { return AggregationServiceURL; }
        }

        int ITracingConfig.MaxBufferedSpans
        {
            get { return MaxBufferedSpans; }
        }

        TimeSpan ITracingConfig.BufferFlushPeriod
        {
            get { return BufferFlushPeriod; }
        }

        TimeSpan ITracingConfig.BufferFlushTimeout
        {
            get { return BufferFlushTimeout; }
        }

        double ITracingConfig.SamplingChance
        {
            get { return SamplingChance; }
        }

        int ITracingConfig.MaxSamplesPerSecond
        {
            get { return MaxSamplesPerSecond; }
        }
    }
}