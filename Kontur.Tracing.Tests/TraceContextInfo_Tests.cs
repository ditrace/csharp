using System;
using FluentAssertions;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing
{
    [TestFixture]
    internal class TraceContextInfo_Tests
    {
        [SetUp]
        public void SetUp()
        {
            traceContextInfo = new TraceContextInfo(true, "traceId", "contextId", "contextName", "parentContextId");
        }

        [Test]
        public void SetAnnotation_should_overwrite_existing_value()
        {
            traceContextInfo.SetAnnotation("key", "value1");
            traceContextInfo.SetAnnotation("key", "value2");

            traceContextInfo.Annotations.Should().Contain("key", "value2");
        }

        [Test]
        public void SetTimepoint_should_overwrite_existing_value()
        {
            var time1 = DateTime.UtcNow;
            var time2 = DateTime.UtcNow.AddSeconds(1);

            traceContextInfo.SetTimepoint("point", time1);
            traceContextInfo.SetTimepoint("point", time2);

            traceContextInfo.Timeline.Should().Contain("point", time2);
        }

        private TraceContextInfo traceContextInfo;
    }
}