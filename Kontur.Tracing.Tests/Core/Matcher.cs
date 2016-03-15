using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing.Core
{
    internal class Matcher
    {
        public static void Match(TraceContextInfo[] realInfos, TraceContextDescriptor[] expectedInfos, IAnnotationNameMapper mapper)
        {
            new Matcher(mapper).Match(realInfos, expectedInfos);
        }

        private Matcher(IAnnotationNameMapper mapper)
        {
            this.mapper = mapper;
        }

        private void Match(TraceContextInfo[] realInfos, TraceContextDescriptor[] expectedInfos)
        {
            Assert.That(realInfos.Length, Is.EqualTo(expectedInfos.Length), "Не совпадают размеры массивов");

            allTimepoints = realInfos.SelectMany(info => info.Timeline.Values).OrderBy(t => t).ToList();

            for (var i = 0; i < realInfos.Length; ++i)
                Match(realInfos[i], expectedInfos[i]);
        }

        private void Match(TraceContextInfo realInfo, TraceContextDescriptor expectedInfo)
        {
            Assert.That(Match(realInfo.TraceId, expectedInfo.TraceId), Is.True, "Элементы {0} и {1} не совпали по полю TraceId", InfoToString(realInfo), DescriptorToString(expectedInfo));
            Assert.That(Match(realInfo.ContextId, expectedInfo.ContextId), Is.True, "Элементы {0} и {1} не совпали по полю ContextId", InfoToString(realInfo), DescriptorToString(expectedInfo));
            Assert.That(Match(realInfo.ParentContextId, expectedInfo.ParentContextId), Is.True, "Элементы {0} и {1} не совпали по полю ParentContextId", InfoToString(realInfo), DescriptorToString(expectedInfo));
            Assert.That(Match(realInfo.ContextName, expectedInfo.ContextName), Is.True, "Элементы {0} и {1} не совпали по полю ContextName", InfoToString(realInfo), DescriptorToString(expectedInfo));
            Assert.That(Match(realInfo.IsRoot, expectedInfo.IsRoot), Is.True, "Элементы {0} и {1} не совпали по полю IsRoot", InfoToString(realInfo), DescriptorToString(expectedInfo));
            Assert.That(Match(realInfo.Annotations, expectedInfo.Annotations), Is.True, "Элементы {0} и {1} не совпали по полю Annotations", InfoToString(realInfo), DescriptorToString(expectedInfo));
            Assert.That(Match(realInfo.Timeline, expectedInfo.Timeline), Is.True, "Элементы {0} и {1} не совпали по полю Timeline", InfoToString(realInfo), DescriptorToString(expectedInfo));
        }

        // Сравнение значений
        // $varname - объявление переменной
        // * - любое значение, в том числе empty и null
        // ? - любое значение, кроме empty и null
        private bool Match(string realValue, string expectedValue)
        {
            if (expectedValue == null) return realValue == null;
            if (expectedValue == "*") return true;
            if (expectedValue == "?") return !string.IsNullOrEmpty(realValue);
            if (!expectedValue.StartsWith("$")) return realValue.Equals(expectedValue, StringComparison.InvariantCultureIgnoreCase);

            var variableName = expectedValue.Substring(1);
            string expectedVariableValue;
            if (variables.TryGetValue(variableName, out expectedVariableValue))
                return string.Equals(realValue, expectedVariableValue, StringComparison.InvariantCultureIgnoreCase);

            variables.Add(variableName, realValue);
            return true;
        }

        // Сравнение аннотаций
        private bool Match(Dictionary<string, string> realAnnotations, Dictionary<Annotation, string> expectedAnnotations)
        {
            if (expectedAnnotations == null) return true;
            if (realAnnotations.Count != expectedAnnotations.Count) return false;
            foreach (var expectedAnnotation in expectedAnnotations)
            {
                var annotationName = mapper.Map(expectedAnnotation.Key);
                string realValue;
                if (!realAnnotations.TryGetValue(annotationName, out realValue)) return false;
                if (!Match(realValue, expectedAnnotation.Value)) return false;
            }
            return true;
        }

        // Сравнение таймлайнов
        private bool Match(Dictionary<string, DateTime> realTimeline, Dictionary<Timepoint, int> expectedTimeline)
        {
            if (expectedTimeline == null) return true;
            if (realTimeline.Count != expectedTimeline.Count) return false;
            foreach (var expectedTimepoint in expectedTimeline)
            {
                var pointName = mapper.Map(expectedTimepoint.Key);
                DateTime realValue;
                if (!realTimeline.TryGetValue(pointName, out realValue)) return false;
                if (expectedTimepoint.Value < 1 || expectedTimepoint.Value > allTimepoints.Count) return false;
                if (!Match(realValue, allTimepoints[expectedTimepoint.Value-1])) return false;
            }
            return true;
        }

        private bool Match(DateTime realTime, DateTime expectedTime)
        {
            return realTime.Equals(expectedTime);
        }

        private bool Match(bool realValue, bool expectedValue)
        {
            return realValue == expectedValue;
        }

        private readonly IAnnotationNameMapper mapper;
        private readonly Dictionary<string, string> variables = new Dictionary<string, string>();
        private List<DateTime> allTimepoints;

        #region FormatOutput

        private string InfoToString(TraceContextInfo info)
        {
            return string.Format("[TraceId={0}, ContextId={1}, ParentContextId={2}, ContextName={3}, IsRoot={4}, Annotations={5}, Timeline={6}]",
                info.TraceId, info.ContextId, info.ParentContextId, info.ContextName, info.IsRoot, DictionaryToString(info.Annotations), DictionaryToString(info.Timeline));
        }

        private string DescriptorToString(TraceContextDescriptor descriptor)
        {
            return string.Format("[TraceId={0}, ContextId={1}, ParentContextId={2}, ContextName={3}, IsRoot={4}, Annotations={5}, Timeline={6}]",
                descriptor.TraceId, descriptor.ContextId, descriptor.ParentContextId, descriptor.ContextName, descriptor.IsRoot, DictionaryToString(descriptor.Annotations), DictionaryToString(descriptor.Timeline));
        }

        private string DictionaryToString(Dictionary<string, string> dictionary)
        {
            var builder = new StringBuilder();
            builder.Append('[');
            dictionary.ToList().ForEach(a => builder.AppendKeyValuePair(a.Key, a.Value));
            builder.Append(']');
            return builder.ToString();
        }

        private object DictionaryToString(Dictionary<Annotation, string> dictionary)
        {
            if (dictionary == null) return "[]";
            var builder = new StringBuilder();
            builder.Append('[');
            dictionary.ToList().ForEach(a => builder.AppendKeyValuePair(mapper.Map(a.Key), a.Value));
            builder.Append(']');
            return builder.ToString();
        }

        private object DictionaryToString(Dictionary<string, DateTime> dictionary)
        {
            var builder = new StringBuilder();
            builder.Append('[');
            dictionary.ToList().ForEach(a => builder.AppendKeyValuePair(a.Key, a.Value.ToString("O")));
            builder.Append(']');
            return builder.ToString();
        }

        private object DictionaryToString(Dictionary<Timepoint, int> dictionary)
        {
            if (dictionary == null) return "[]";
            var builder = new StringBuilder();
            builder.Append('[');
            dictionary.ToList().ForEach(a => builder.AppendKeyValuePair(mapper.Map(a.Key), a.Value.ToString()));
            builder.Append(']');
            return builder.ToString();
        }

        #endregion
    }
}