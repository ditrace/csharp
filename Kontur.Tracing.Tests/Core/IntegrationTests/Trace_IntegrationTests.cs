using System.Collections.Generic;
using Kontur.Tracing.Core.TraceCases;
using Kontur.Tracing.Core.TraceCases.Edi;
using NUnit.Framework;

namespace Kontur.Tracing.Core.IntegrationTests
{
    [TestFixture, Category("Functional")]
    public class Trace_IntegrationTests
    {
        private readonly IntegrationTraceTester traceTester = new IntegrationTraceTester();

        [Test]
        public void Test_RootContext()
        {
            traceTester.Run(CommonTraceCases.RootCase, new[]
            {
                new TraceContextDescriptor {TraceId = "?", ContextId = "?", ParentContextId = null, ContextName = "Root", IsRoot = true}
            });
        }

        [Test]
        public void Test_RootChildContext()
        {
            traceTester.Run(CommonTraceCases.RootChildCase, new[]
            {
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$rootId", ParentContextId = null, ContextName = "Root", IsRoot = true},
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$childId", ParentContextId = "$rootId", ContextName = "Child"},
            });
        }

        [Test]
        public void Test_RootChildContinueContext()
        {
            traceTester.Run(CommonTraceCases.RootChildContinueCase, new[]
            {
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$rootId", ParentContextId = null, ContextName = "Root", IsRoot = true},
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$childId", ParentContextId = "$rootId", ContextName = "Child"},
            });
        }

        [Test]
        public void Test_RootChildContinueFinish()
        {
            traceTester.Run(CommonTraceCases.RootChildContinueFinishCase, new[]
            {
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$rootId", ParentContextId = null, ContextName = "Root", IsRoot = true},
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$childId", ParentContextId = "$rootId", ContextName = "Child"},
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$childWithContinueId", ParentContextId = "$rootId", ContextName = "ChildWithContinue"},
                new TraceContextDescriptor {TraceId = "$traceId", ContextId = "$childWithFinishId", ParentContextId = "$rootId", ContextName = "ChildWithFinish"},
            });
        }

        [Test]
        public void Test_ClientServerCase()
        {
            traceTester.Run(CommonTraceCases.ClientServerCase, new[]
            {
                new TraceContextDescriptor
                {
                    TraceId = "$traceId", ContextId = "$contextId", ParentContextId = null, ContextName = "Client", IsRoot = true,
                    Annotations = new Dictionary<Annotation, string>
                    {
                        {Annotation.RequestUrl, "?"},
                    },
                    Timeline = new Dictionary<Timepoint, int>
                    {
                        {Timepoint.ClientSend, 1},
                        {Timepoint.ServerReceive, 2},
                        {Timepoint.ServerSend, 3},
                        {Timepoint.ClientReceive, 4}
                    }
                },
            });
        }

        [Test]
        public void Test_AsyncTaskOnServer()
        {
            traceTester.Run(EdiTraceCases.AsyncTaskOnServer, new[]
            {
                new TraceContextDescriptor
                {
                    TraceId = "$traceId", ContextId = "$csContextId", ParentContextId = null, ContextName = "Client-Server", IsRoot = true,
                    Annotations = new Dictionary<Annotation, string>
                    {
                        {Annotation.RequestUrl, "$url"},
                    },
                    Timeline = new Dictionary<Timepoint, int>
                    {
                        {Timepoint.ClientSend, 1},
                        {Timepoint.ServerReceive, 2},
                        {Timepoint.ServerSend, 5},
                        {Timepoint.ClientReceive, 6},
                    }
                },
                new TraceContextDescriptor
                {
                    TraceId = "$traceId", ContextId = "$taskId", ParentContextId = "$csContextId", ContextName = "TaskLife",
                    Timeline = new Dictionary<Timepoint, int>
                    {
                        {Timepoint.Start, 3},
                        {Timepoint.Finish, 10},
                    }
                },
                new TraceContextDescriptor
                {
                    TraceId = "$traceId", ContextId = "$queueContextId", ParentContextId = "$taskId", ContextName = "Queue",
                    Timeline = new Dictionary<Timepoint, int>
                    {
                        {Timepoint.Start, 4},
                        {Timepoint.Finish, 7},
                    }
                },
                new TraceContextDescriptor
                {
                    TraceId = "$traceId", ContextId = "$handleContextId", ParentContextId = "$taskId", ContextName = "Handling",
                    Timeline = new Dictionary<Timepoint, int>
                    {
                        {Timepoint.Start, 8},
                        {Timepoint.Finish, 9}
                    }
                },
            });
        }
    }
}