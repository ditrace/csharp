using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing
{
    [TestFixture]
    internal class TimeProvider_Tests
    {
        [SetUp]
        public void TestSetup()
        {
            provider = new TimeProvider();
        }

        [Test]
        public void Should_provide_ticks_level_resolution()
        {
            var timestamp = Stopwatch.GetTimestamp();
            var observedTimes = new List<DateTime>();

            for (int i = 0; i < 100000; i++)
            {
                while (Stopwatch.GetTimestamp() == timestamp)
                {
                }

                observedTimes.Add(provider.GetCurrentTime());

                timestamp = Stopwatch.GetTimestamp();
            }

            observedTimes.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void Should_provide_monotonic_increase_of_returned_time()
        {
            var previous = provider.GetCurrentTime();

            for (int i = 0; i < 1000000; i++)
            {
                var current = provider.GetCurrentTime();

                current.Should().BeOnOrAfter(previous);

                previous = current;
            }
        }

        private TimeProvider provider;
    }
}