using System;

namespace Kontur.Tracing.Core.Config
{
    public interface ITracingConfig
    {
        /// <summary>
        /// Trigger for enabling/disabling tracing
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Project identifier
        /// </summary>
        string AggregationServiceSystem { get; }

        /// <summary>
        /// DiTrace gate address
        /// </summary>
        string AggregationServiceUrl { get; }

        /// <summary>
        /// Maximum count of spans that can be saved in memory before sending
        /// </summary>
        int MaxBufferedSpans { get; }

        /// <summary>
        /// Spans buffer flushing period
        /// </summary>
        TimeSpan BufferFlushPeriod { get; }

        /// <summary>
        /// Flush requests timeout
        /// </summary>
        TimeSpan BufferFlushTimeout { get; }

        /// <summary>
        /// Probability between 0 and 1 of a chance that trace will be sended to DiTrace gate
        /// </summary>
        double SamplingChance { get; }

        /// <summary>
        /// Maximum traces per second
        /// </summary>
        int MaxSamplesPerSecond { get; }
    }
}