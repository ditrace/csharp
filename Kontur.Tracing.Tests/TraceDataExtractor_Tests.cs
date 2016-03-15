using System;
using System.Collections.Specialized;
using NUnit.Framework;
using log4net;

namespace Kontur.Tracing
{
    [TestFixture]
    internal class TraceDataExtractor_Tests
    {
        ILog logger = LogManager.GetLogger(typeof(TraceDataExtractor_Tests).Name);
        
        [SetUp]
        public void TestSetup()
        {
            headers = new NameValueCollection();
        }

        [Test]
        public void Should_extract_nulls_from_empty_headers()
        {
            Extract();

            Assert.That(traceId, Is.Null);
            Assert.That(traceSpanId, Is.Null);
            Assert.That(traceParentSpanId, Is.Null);
            Assert.That(traceProfileId, Is.Null);
            Assert.That(traceSampled, Is.Null);
        }

        [Test]
        public void Should_extract_nulls_when_trace_id_is_not_defined()
        {
            headers[TraceHttpHeaders.XKonturTraceParentSpanId] = Guid.NewGuid().ToString();
            headers[TraceHttpHeaders.XKonturTraceProfileId] = Guid.NewGuid().ToString();

            Extract();

            Assert.That(traceId, Is.Null);
            Assert.That(traceSpanId, Is.Null);
            Assert.That(traceParentSpanId, Is.Null);
            Assert.That(traceProfileId, Is.Null);
            Assert.That(traceSampled, Is.Null);
        }

        [Test]
        public void Should_extract_trace_id_when_other_headers_are_not_present()
        {
            headers[TraceHttpHeaders.XKonturTraceId] = "123";

            Extract();

            Assert.That(traceId, Is.EqualTo("123"));
            Assert.That(traceParentSpanId, Is.Null);
            Assert.That(traceProfileId, Is.Null);
            Assert.That(traceSampled, Is.False);
        }

        [Test]
        public void Should_extract_trace_span_id()
        {
            headers[TraceHttpHeaders.XKonturTraceId] = "123";
            headers[TraceHttpHeaders.XKonturTraceSpanId] = "456";

            Extract();

            Assert.That(traceSpanId, Is.EqualTo("456"));
        }

        [Test]
        public void Should_extract_trace_parent_span_id()
        {
            headers[TraceHttpHeaders.XKonturTraceId] = "123";
            headers[TraceHttpHeaders.XKonturTraceParentSpanId] = "456";

            Extract();

            Assert.That(traceParentSpanId, Is.EqualTo("456"));
        }

        [Test]
        public void Should_extract_trace_profile_id()
        {
            headers[TraceHttpHeaders.XKonturTraceId] = "123";
            headers[TraceHttpHeaders.XKonturTraceProfileId] = "456";

            Extract();

            Assert.That(traceProfileId, Is.EqualTo("456"));
        }

        [Test]
        public void Should_extract_true_for_sampling_flag_when_corresponding_header_exists()
        {
            headers[TraceHttpHeaders.XKonturTraceId] = "123";
            headers[TraceHttpHeaders.XKonturTraceIsSampled] = "true";

            Extract();

            Assert.That(traceSampled, Is.True);
        }

        private void Extract()
        {
            TraceDataExtractor.ExtractFromHttpHeaders(headers, out traceId, out traceSpanId, out traceParentSpanId, out traceProfileId, out traceSampled, logger);
        }

        private NameValueCollection headers;

        private string traceId;
        private string traceSpanId;
        private string traceParentSpanId;
        private string traceProfileId;
        private bool? traceSampled;
    }
}