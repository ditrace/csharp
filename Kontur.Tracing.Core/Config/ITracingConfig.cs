using System;

namespace Kontur.Tracing.Core.Config
{
    public interface ITracingConfig
    {
        /// <summary>
        /// –убильник дл€ включени€/выключени€ трассировки
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// ѕродуктовый идентификатор дл€ запросов к сервису-агрегатору трассировок
        /// </summary>
        string AggregationServiceSystem { get; }

        /// <summary>
        /// јдрес дл€ запросов к сервису-агрегатору трассировок
        /// </summary>
        string AggregationServiceUrl { get; }

        /// <summary>
        /// ћаксимальное кол-во span'ов, которое может быть сохранено в пам€ти дл€ отправки
        /// </summary>
        int MaxBufferedSpans { get; }

        /// <summary>
        /// ѕериод, с которым накопленные span'ы отправл€ютс€ на сервис-агрегатор трассировок
        /// </summary>
        TimeSpan BufferFlushPeriod { get; }

        /// <summary>
        /// “аймаут на запрос по отправке накопленных span'ов на сервис-агрегатор трассировок
        /// </summary>
        TimeSpan BufferFlushTimeout { get; }

        /// <summary>
        /// ¬еро€тность от 0 до 1, с которой кажда€ нова€ трассировка получает шанс быть сохраненной
        /// </summary>
        double SamplingChance { get; }

        /// <summary>
        /// ћаксимальное количество семплируемых трассировок в секунду
        /// </summary>
        int MaxSamplesPerSecond { get; }
    }
}