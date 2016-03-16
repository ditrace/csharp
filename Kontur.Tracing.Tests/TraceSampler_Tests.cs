using System;
using FluentAssertions;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing
{
    [TestFixture]
    internal class TraceSampler_Tests
    {
        [SetUp]
        public void SetUp()
        {
            config = new Core.Config.TracingConfig(true, null, null)
            {
                SamplingChance = 1d,
                MaxSamplesPerSecond = int.MaxValue,
            };

            timeProvider = new TestTimeProvider{
                Value = DateTime.UtcNow
            };

            sampler = new Core.Impl.TraceSampler(new StaticConfigurationProvider(config), timeProvider);
        }

        [Test]
        public void Should_enforce_sampling_chance_restriction()
        {
            config.SamplingChance = 0.25;

            const int probes = 50000;

            int allowed = 0;

            for (int i = 0; i < probes; i++)
                if (sampler.CanSampleTrace())
                    allowed++;

            var allowedFraction = (double)allowed/probes;

            allowedFraction.Should().BeApproximately(0.25, 0.01);
        }

        [Test]
        public void Should_enforce_max_samples_per_second_restriction()
        {
            config.MaxSamplesPerSecond = 2;

            sampler.CanSampleTrace().Should().BeTrue();

            ShiftCurrentTime(500.Milliseconds());
            sampler.CanSampleTrace().Should().BeTrue();

            ShiftCurrentTime(250.Milliseconds());
            sampler.CanSampleTrace().Should().BeFalse();
            sampler.CanSampleTrace().Should().BeFalse();

            ShiftCurrentTime(500.Milliseconds());
            sampler.CanSampleTrace().Should().BeTrue();
            sampler.CanSampleTrace().Should().BeFalse();

            ShiftCurrentTime(2.Seconds());
            sampler.CanSampleTrace().Should().BeTrue();
            sampler.CanSampleTrace().Should().BeTrue();
        }

        private void ShiftCurrentTime(TimeSpan delta)
        {
            timeProvider.Value = timeProvider.GetCurrentTime() + delta;
        }

        private Core.Config.TracingConfig config;
        private TestTimeProvider timeProvider;
        private Core.Impl.TraceSampler sampler;
    }

    class TestTimeProvider : ITimeProvider
    {
        public DateTime Value {get; set;}
        
        
        public DateTime GetCurrentTime()
        {
            return Value;
        }
    }
}