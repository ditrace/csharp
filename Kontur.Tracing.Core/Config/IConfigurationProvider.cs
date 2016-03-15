using JetBrains.Annotations;

namespace Kontur.Tracing.Core.Config
{
    public interface IConfigurationProvider
    {
        [NotNull]
        ITracingConfig GetConfig();
    }
}