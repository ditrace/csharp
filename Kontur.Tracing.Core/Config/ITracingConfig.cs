using System;

namespace Kontur.Tracing.Core.Config
{
    public interface ITracingConfig
    {
        /// <summary>
        /// ��������� ��� ���������/���������� �����������
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// ����������� ������������� ��� �������� � �������-���������� �����������
        /// </summary>
        string AggregationServiceSystem { get; }

        /// <summary>
        /// ����� ��� �������� � �������-���������� �����������
        /// </summary>
        string AggregationServiceUrl { get; }

        /// <summary>
        /// ������������ ���-�� span'��, ������� ����� ���� ��������� � ������ ��� ��������
        /// </summary>
        int MaxBufferedSpans { get; }

        /// <summary>
        /// ������, � ������� ����������� span'� ������������ �� ������-��������� �����������
        /// </summary>
        TimeSpan BufferFlushPeriod { get; }

        /// <summary>
        /// ������� �� ������ �� �������� ����������� span'�� �� ������-��������� �����������
        /// </summary>
        TimeSpan BufferFlushTimeout { get; }

        /// <summary>
        /// ����������� �� 0 �� 1, � ������� ������ ����� ����������� �������� ���� ���� �����������
        /// </summary>
        double SamplingChance { get; }

        /// <summary>
        /// ������������ ���������� ������������ ����������� � �������
        /// </summary>
        int MaxSamplesPerSecond { get; }
    }
}