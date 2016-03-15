using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Config
{
    public class StaticConfigurationProvider : IConfigurationProvider
    {
        public static readonly IConfigurationProvider Disabled = new StaticConfigurationProvider(new TracingConfig(false, string.Empty, string.Empty));

        public StaticConfigurationProvider([NotNull] TracingConfig tracingConfig)
        {
            this.tracingConfig = tracingConfig;
        }

        [NotNull]
        public ITracingConfig GetConfig()
        {
            return tracingConfig;
        }

        private readonly TracingConfig tracingConfig;
    }
}