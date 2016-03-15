using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Impl;
using Nest;
using Newtonsoft.Json.Linq;

namespace Kontur.Tracing.Core.IntegrationTests
{
    public class IntegrationTraceTester
    {
        private const string testSystemHost = "vm-elastic";
        private const string aggregationServiceSystem = "tracing-tests";
        private readonly TimeSpan waitingTime = TimeSpan.FromSeconds(40);
        private readonly TimeSpan queryInterval = TimeSpan.FromSeconds(2);

        public void Run(Action traceAction, TraceContextDescriptor[] expectedInfos)
        {
            var configProvider = new StaticConfigurationProvider(new Config.TracingConfig(true, aggregationServiceSystem, string.Format("http://{0}:9003/spans", testSystemHost))
            {
                SamplingChance = 1D,
            });
            var environment = new TracingEnvironment();

            Trace.Initialize(configProvider, environment);
            var startTime = DateTime.UtcNow;
            try
            {
                traceAction();
            }
            finally
            {
                Trace.Stop();
            }

            expectedInfos = expectedInfos
                .OrderBy(x => (x.Timeline == null || x.Timeline.Count == 0) ? int.MinValue : x.Timeline.Select(pair => pair.Value).Min())
                .ToArray();

            var stopWatch = Stopwatch.StartNew();
            while(stopWatch.Elapsed < waitingTime)
            {
                List<TraceContextInfo> infos;
                if (TryGetInfos(startTime, out infos))
                {
                    try
                    {
                        Matcher.Match(infos.ToArray(), expectedInfos, environment.AnnotationNameMapper);
                        Console.WriteLine("Query, time: {0}, success: true", stopWatch.Elapsed);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Query, time: {0}, success: false, Exception: {1}", stopWatch.Elapsed, ex.Message);
                    }
                }
                else Console.WriteLine("Query #{0}, success: false, Exception: Не удалось получить или десериализовать информацию с сервера");
                Thread.Sleep(queryInterval);
            }

            throw new Exception(string.Format("Условие не выполнилось за {0}", waitingTime));
        }

        private bool TryGetInfos(DateTime startTime, out List<TraceContextInfo> infos)
        {
            var settings = new ConnectionSettings(new Uri(string.Format("http://{0}:9200/", testSystemHost)), string.Format("traces-{0}", DateTime.UtcNow.ToString("yyyy.MM.dd")));
            var client = new ElasticClient(settings);

            infos = new List<TraceContextInfo>();
            try
            {
                var results = client.Search<JToken>(body =>
                    body.AllTypes().Take(100).Query(d =>
                        d.Bool(b => b.Must(y => y.Range(z => z.OnField("timestamp").GreaterOrEquals(startTime)), x => x.Term("system", aggregationServiceSystem)))));

                foreach (var document in results.Documents)
                {
                    var traceId = GetString(document["id"]);
                    var spans = (JArray)document["spans"];
                    infos.AddRange(spans.Select(span => Deserialize(span, traceId)));
                }

                // Сортируем по первой метке времени
                infos = infos.OrderBy(info => info.Timeline.Count == 0 ? new DateTime() : info.Timeline.Select(t => t.Value).Min()).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошла ошибка во время получения спанов в сервера: {0}", e);
                return false;
            }
            return true;
        }

        private static TraceContextInfo Deserialize(JToken span, string traceId)
        {
            var annotationNames = typeof (Annotation)
                .GetFields()
                .Select(field => field.GetCustomAttributes(typeof (StringValueAttribute), false).FirstOrDefault())
                .OfType<StringValueAttribute>()
                .Select(attribute => attribute.Value)
                .ToList();

            var annotations = annotationNames
                .Where(annotationName => GetString(span[annotationName]) != null)
                .ToDictionary(annotationName => annotationName, annotationName => GetString(span[annotationName].ToString()));

            var timeline = (span["timeline"] == null) ? new Dictionary<string, DateTime>() : ((JObject)span["timeline"])
                .Properties()
                .ToDictionary(prop => prop.Name, prop => (DateTime)prop.Value);

            var isRoot = GetString(span["root"]) == "true";
            var contextId = GetString(span["spanid"]);
            var contextName = GetString(span["targetid"]);
            var parentContextId = GetString(span["parentspanid"]);
            return new TraceContextInfo(isRoot, traceId, contextId, contextName, parentContextId)
            {
                Timeline = timeline,
                Annotations = annotations,
            };
        }

        private static string GetString(JToken jToken)
        {
            return jToken == null ? null : jToken.ToString();
        }
    }
}