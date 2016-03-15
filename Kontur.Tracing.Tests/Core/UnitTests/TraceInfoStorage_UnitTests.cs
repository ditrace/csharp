using System;
using FluentAssertions;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing.Core.UnitTests
{
    [TestFixture]
    internal class TraceInfoStorage_Tests
    {
        [SetUp]
        public void TestSetup()
        {
            var config = new Config.TracingConfig(true, string.Empty, string.Empty)
            {
                MaxBufferedSpans = 3,
            };
            var configProvider = new StaticConfigurationProvider(config);
            storage = new TraceInfoStorage(configProvider);
        }

        [Test]
        public void TryAdd_should_not_allow_to_exceed_buffering_limit()
        {
            storage.TryAdd(GenerateContextInfo()).Should().BeTrue();
            storage.TryAdd(GenerateContextInfo()).Should().BeTrue();
            storage.TryAdd(GenerateContextInfo()).Should().BeTrue();

            storage.TryAdd(GenerateContextInfo()).Should().BeFalse();
            storage.TryAdd(GenerateContextInfo()).Should().BeFalse();
        }

        [Test]
        public void GetAll_should_return_all_buffered_infos()
        {
            var info1 = GenerateContextInfo();
            var info2 = GenerateContextInfo();
            var info3 = GenerateContextInfo();

            storage.TryAdd(info1);
            storage.TryAdd(info2);
            storage.TryAdd(info3);

            storage.GetAll().Should().Equal(info1, info2, info3);
        }

        [Test]
        public void RemoveAll_should_remove_given_infos()
        {
            var info1 = GenerateContextInfo();
            var info2 = GenerateContextInfo();
            var info3 = GenerateContextInfo();

            storage.TryAdd(info1);
            storage.TryAdd(info2);
            storage.TryAdd(info3);

            storage.Remove(new[] {info1, info3});

            storage.GetAll().Should().ContainSingle().Which.Should().BeSameAs(info2);
        }

        [Test]
        public void TryAdd_with_same_contextId_should_not_delete_previous()
        {
            var traceId = TraceIdGenerator.CreateTraceId();
            var contextId = TraceIdGenerator.CreateTraceContextId();

            var info1 = GenerateContextInfo(traceId, contextId);
            var info2 = GenerateContextInfo(traceId, contextId);

            storage.TryAdd(info1);
            storage.TryAdd(info2);

            storage.GetAll().Should().Equal(info1, info2);
        }

        private static TraceContextInfo GenerateContextInfo(string traceId = null, string contextId = null)
        {
            return new TraceContextInfo(false, traceId ?? Guid.NewGuid().ToString(), contextId ?? Guid.NewGuid().ToString(), null, null);
        }

        private TraceInfoStorage storage;
    }
}