using System;
using System.Linq;
using Kontur.Tracing.Core;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing
{
    [TestFixture]
    internal class AnnotationNameMapper_Tests
    {
        [SetUp]
        public void TestSetup()
        {
            mapper = new AnnotationNameMapper();
        }

        [Test]
        public void Should_cover_all_annotations()
        {
            foreach (var annotation in Enum.GetValues(typeof(Annotation)).Cast<Annotation>())
            {
                mapper.Map(annotation);
            }
        }

        [Test]
        public void Should_cover_all_timepoints()
        {
            foreach (var timepoint in Enum.GetValues(typeof(Timepoint)).Cast<Timepoint>())
            {
                mapper.Map(timepoint);
            }
        }

        private AnnotationNameMapper mapper;
    }
}