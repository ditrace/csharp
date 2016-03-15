using Kontur.Tracing.Core.Impl;

namespace Kontur.Tracing.Core
{
    public enum Annotation
    {
        [StringValue("url_method")] RequestMethod,
        [StringValue("url")] RequestUrl,
        [StringValue("host")] RequestHost,
        [StringValue("targetId")] RequestTargetId,
        [StringValue("rqbl")] RequestBodyLength,
        [StringValue("rc")] ResponseCode,
        [StringValue("rsbl")] ResponseBodyLength,
        [StringValue("wrapper")] Wrapper,
        [StringValue("revision")] Revision,
    }
}