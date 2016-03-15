using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.Tracing.Core.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Kontur.Tracing
{
    [TestFixture]
    internal class TraceInfoSerializer_Tests
    {
        [SetUp]
        public void SetUp()
        {
            serializer = new TraceInfoSerializer();
        }

        [Test]
        public void Should_correctly_serialize_a_span_without_optional_identifiers()
        {
            SerializeAndVerify(new TraceContextInfo(false, "traceId", "contextId", null, null));
        }

        [Test]
        public void Should_correctly_serialize_a_span_with_optional_identifiers()
        {
            SerializeAndVerify(new TraceContextInfo(true, "traceId", "contextId", "contextName", "parentContextId"));
        }

        [Test]
        public void Should_correctly_serialize_a_span_with_annotations_and_timeline()
        {
            var traceContextInfo = new TraceContextInfo(true, "traceId", "contextId", "contextName", "parentContextId");
            traceContextInfo.Annotations.Add("k1", "v1");
            traceContextInfo.Annotations.Add("k2", "v2");
            traceContextInfo.Timeline.Add("k1", DateTime.UtcNow);
            traceContextInfo.Timeline.Add("k2", DateTime.UtcNow.AddSeconds(1));
            SerializeAndVerify(traceContextInfo);
        }

        private void SerializeAndVerify(TraceContextInfo traceContextInfo)
        {
            var json = serializer.Serialize(new []{traceContextInfo});
            var spanDto = DeserializeSingleItem(json);
            AssertSpanDto(spanDto, traceContextInfo);
        }

        [Test]
        public void Should_correctly_serialize_multiple_spans()
        {
            var traceContextInfo1 = new TraceContextInfo(true, "traceId1", "contextId1", "contextName1", "parentContextId");
            var traceContextInfo2 = new TraceContextInfo(false, "traceId2", "contextId2", "contextName2", "parentContextId2");
            var json = serializer.Serialize(new[] {traceContextInfo1, traceContextInfo2});
            var actual = Deserialize(json);
            Assert.That(actual.Count, Is.EqualTo(2));
            AssertSpanDto(actual[0], traceContextInfo1);
            AssertSpanDto(actual[1], traceContextInfo2);
        }

        private static void AssertSpanDto(SpanDto actual, TraceContextInfo expected)
        {
            Assert.That(actual.TraceId, Is.EqualTo(expected.TraceId));
            Assert.That(actual.SpanId, Is.EqualTo(expected.ContextId));
            Assert.That(actual.ParentSpanId, Is.EqualTo(expected.ParentContextId));
            Assert.That(actual.Timeline, Is.EquivalentTo(expected.Timeline));
            Assert.That(actual.Annotations.Where(x => x.Key != "root" && x.Key != "targetId").ToList(), Is.EquivalentTo(expected.Annotations));
            string isRootStr;
            var actualIsRoot = actual.Annotations.TryGetValue("root", out isRootStr) && bool.Parse(isRootStr);
            Assert.That(actualIsRoot, Is.EqualTo(expected.IsRoot));
            string actualContextName;
            if (actual.Annotations.TryGetValue("targetId", out actualContextName))
                Assert.That(actualContextName, Is.EqualTo(expected.ContextName));
            else
                Assert.That(expected.ContextName == null);
        }

        [Test]
        public void Should_produce_same_results_when_being_reused()
        {
            var traceContextInfo = new TraceContextInfo(true, "traceId", "contextId", "contextName", "parentContextId");
            var json1 = serializer.Serialize(new []{traceContextInfo});
            var json2 = serializer.Serialize(new []{traceContextInfo});
            Assert.That(json1, Is.EqualTo(json2));
        }

        [Test]
        public void Should_not_use_contextName_if_annotations_contains_targetId()
        {
            var traceContextInfo = new TraceContextInfo(true, "traceId", "contextId", "contextName", "parentContextId");
            traceContextInfo.Annotations.Add("targetId", "otherContextName");
            var json = serializer.Serialize(new []{traceContextInfo});
            var actual = DeserializeSingleItem(json);
            Assert.That(actual.Annotations["targetId"], Is.EqualTo("otherContextName"));
        }

        private static SpanDto DeserializeSingleItem(string json)
        {
            Console.Out.WriteLine(json);
            var spanDto = Deserialize(json).Single();
            Console.Out.WriteLine(JsonConvert.SerializeObject(spanDto));
            return spanDto;
        }

        private static List<SpanDto> Deserialize(string json)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "O",
                ContractResolver = new DefaultContractResolver()
            };
            return json
                .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => JsonConvert.DeserializeObject<SpanDto>(x, settings))
                .ToList();
        }

        private TraceInfoSerializer serializer;
    }
}