using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Impl
{
    internal class AnnotationNameMapper : IAnnotationNameMapper
    {
        public AnnotationNameMapper()
        {
            annotationNames = new Dictionary<Annotation, string>();
            timepointNames = new Dictionary<Timepoint, string>();

            foreach (var field in typeof(Annotation).GetFields())
            {
                var attribute = field.GetCustomAttributes(typeof (StringValueAttribute), false).FirstOrDefault() as StringValueAttribute;
                if (attribute == null)
                    continue;

                Annotation annotation;
                if (!Enum.TryParse(field.Name, out annotation))
                    continue;

                annotationNames[annotation] = attribute.Value;
            }

            foreach (var field in typeof (Timepoint).GetFields())
            {
                var attribute = field.GetCustomAttributes(typeof (StringValueAttribute), false).FirstOrDefault() as StringValueAttribute;
                if (attribute == null)
                    continue;

                Timepoint timepoint;
                if (!Enum.TryParse(field.Name, out timepoint))
                    continue;

                timepointNames[timepoint] = attribute.Value;
            }
        }

        [NotNull]
        public string Map(Annotation annotation)
        {
            return annotationNames[annotation];
        }

        [NotNull]
        public string Map(Timepoint timepoint)
        {
            return timepointNames[timepoint];
        }

        private readonly Dictionary<Annotation, string> annotationNames;
        private readonly Dictionary<Timepoint, string> timepointNames;
    }
}