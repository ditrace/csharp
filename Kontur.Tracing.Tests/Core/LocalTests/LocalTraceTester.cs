using System;
using System.Linq;
using Kontur.Tracing.Core.Config;

namespace Kontur.Tracing.Core.FunctionalTests
{
    public class LocalTraceTester
    {
        public void Run(Action traceAction, TraceContextDescriptor[] expectedInfos)
        {
            var configProvider = new StaticConfigurationProvider(new Config.TracingConfig(true, string.Empty, string.Empty)
            {
                SamplingChance = 1D,
            });
            var environment = new TestTracingEnvironment();

            Trace.Initialize(configProvider, environment);
            try
            {
                traceAction();
            }
            finally
            {
                Trace.Stop();
            }
            var realInfos = environment.TraceInfoStorage.GetAll();
            Matcher.Match(realInfos.ToArray(), expectedInfos, environment.AnnotationNameMapper);
        }
    }
}