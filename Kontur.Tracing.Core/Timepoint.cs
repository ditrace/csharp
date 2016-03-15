using Kontur.Tracing.Core.Impl;

namespace Kontur.Tracing.Core
{
    public enum Timepoint
    {
        [StringValue("cs")] ClientSend,
        [StringValue("cr")] ClientReceive,
        [StringValue("sr")] ServerReceive,
        [StringValue("ss")] ServerSend,
        [StringValue("sr")] Start,
        [StringValue("ss")] Finish,
    }
}