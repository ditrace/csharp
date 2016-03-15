using System;
using FluentAssertions;
using Kontur.Tracing.Core.Config;
using Kontur.Tracing.Core.Impl;
using NUnit.Framework;

namespace Kontur.Tracing.Core.UnitTests
{
    [TestFixture]
    internal class Trace_UnitTests
    {
        [SetUp]
        public void TestSetup()
        {
            var config = new Config.TracingConfig(true, string.Empty, string.Empty)
            {
                MaxSamplesPerSecond = int.MaxValue,
                SamplingChance = 1D,
            };
            var configProvider = new StaticConfigurationProvider(config);
            var env = new TestTracingEnvironment();
            Trace.Initialize(configProvider, env);
        }

        [TearDown]
        public void TestTeardown()
        {
            Trace.Stop();
        }

        [Test]
        public void CurrentContext_should_be_NoOp()
        {
            TraceContext.Current.Should().BeSameAs(NoOpTraceContext.Instance);
        }

        [Test]
        public void CreateRootContext_should_generate_new_trace_id_and_context_id()
        {
            using (var traceContext = Trace.CreateRootContext("Test"))
            {
                traceContext.TraceId.Should().NotBeNullOrEmpty();
                traceContext.ContextId.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void CreateRootContext_should_return_NoOp_instance_when_current_context_is_not_sampled()
        {
            using (var traceContext = Trace.CreateRootContext("Test", activate: false))
                traceContext.Should().BeSameAs(NoOpTraceContext.Instance);
        }

        [Test]
        public void CreateRootContext_should_return_real_context_when_current_context_is_sampled()
        {
            using (var traceContext = Trace.CreateRootContext("Test"))
                traceContext.Should().NotBeSameAs(NoOpTraceContext.Instance);
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateRootContext_should_throw_exception_if_context_name_is_null_or_empty(string contextName)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var traceContext = Trace.CreateRootContext(contextName))
                {
                }
            });
        }

        [Test]
        public void CreateRootContext_should_throw_exception_if_context_is_set_already()
        {
            using (var rootContext = Trace.CreateRootContext("Test"))
            {
                Assert.DoesNotThrow(() =>
                {
                    using (var newRootContext = Trace.CreateRootContext("NewRoot"))
                    {
                    }
                });
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateChildContext_should_throw_exception_if_context_name_is_null_or_empty(string contextName)
        {
            using (var rootContext = Trace.CreateRootContext("Test"))
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    using (var childContext = Trace.CreateChildContext(contextName))
                    {
                    }
                });
            }
        }

        [Test]
        public void CreateChildContext_should_return_NoOp_instance_if_current_context_is_null()
        {
            using (var childContext = Trace.CreateChildContext("Test"))
                childContext.Should().BeSameAs(NoOpTraceContext.Instance);
        }

        [Test]
        public void CreateChildContext_should_update_context_and_pop_after_dispose()
        {
            using (var rootContext = Trace.CreateRootContext("Test"))
            {
                var originalContext = TraceContext.Current;

                using (var childContext = Trace.CreateChildContext("Child"))
                    childContext.TraceId.Should().BeSameAs(rootContext.TraceId);

                TraceContext.Current.Should().BeSameAs(originalContext);
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public void CreateChildContext_should_generate_context_id(string contextId)
        {
            using (var rootContext = Trace.CreateRootContext("Test"))
            using (var childContext = Trace.CreateChildContext("Child", contextId))
                childContext.TraceId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CreateChildContext_should_set_context_id()
        {
            var contextId = TraceIdGenerator.CreateTraceContextId();
            using (var rootContext = Trace.CreateRootContext("Test"))
            using (var childContext = Trace.CreateChildContext("Child", contextId))
                childContext.ContextId.Should().BeSameAs(contextId);
        }

        [Test]
        public void ContinueContext_should_use_trace_and_context_id_from_parameter_when_active()
        {
            var traceId = TraceIdGenerator.CreateTraceId();
            var contextId = TraceIdGenerator.CreateTraceContextId();
            using (var traceContext = Trace.ContinueContext(traceId, contextId, isActive: true, isRoot: false))
            {
                traceContext.TraceId.Should().BeSameAs(traceId);
                traceContext.ContextId.Should().BeSameAs(contextId);
                traceContext.IsActive.Should().Be(true);
            }
        }

        [Test]
        public void ContinueContext_should_set_null_traceid_and_contextid_when_not_active()
        {
            var traceId = TraceIdGenerator.CreateTraceId();
            var contextId = TraceIdGenerator.CreateTraceContextId();
            using (var traceContext = Trace.ContinueContext(traceId, contextId, isActive: false, isRoot: false))
            {
                traceContext.TraceId.Should().BeSameAs(string.Empty);
                traceContext.ContextId.Should().BeSameAs(string.Empty);
                traceContext.IsActive.Should().Be(false);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ContinueContext_should_throw_exception_if_context_is_set_already(bool isActive)
        {
            var traceId = TraceIdGenerator.CreateTraceId();
            var contextId = TraceIdGenerator.CreateTraceContextId();
            using (var traceContext = Trace.CreateRootContext("Test"))
            {
                Assert.DoesNotThrow(() =>
                {
                    using (var continueContext = Trace.ContinueContext(traceId, contextId, isActive, isRoot: false))
                    {
                    }
                });
            }
        }

        [TestCase(null, "ContextId")]
        [TestCase("TraceId", null)]
        [TestCase(null, null)]
        public void ContinueContext_should_throw_exception_if_trace_id_or_context_id_is_null(string traceId, string contextId)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var continueContext = Trace.ContinueContext(traceId, contextId, isActive: true, isRoot: false))
                {
                }
            });
        }

        [Test]
        public void FinishContext_should_pop_context()
        {
            using (var rootContext = Trace.CreateRootContext("Test"))
            {
                var originalContext = TraceContext.Current;

                var childContext = Trace.CreateChildContext("Child");
                Trace.FinishCurrentContext();

                TraceContext.Current.Should().BeSameAs(originalContext);
            }
        }

        [Test]
        public void CreateChildContext_with_fixed_parent()
        {
            var traceId = TraceIdGenerator.CreateTraceId();
            var parentContextId = TraceIdGenerator.CreateTraceContextId();
            using (var childContext = Trace.CreateChildContext("Child", traceId, parentContextId))
            {
                childContext.Should().NotBeSameAs(NoOpTraceContext.Instance);
                childContext.TraceId.Should().BeSameAs(traceId);
                childContext.IsActive.Should().BeTrue();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Initialize_should_throw_exception_if_already_initialized(bool isEnabled)
        {
            Assert.Throws<InvalidOperationException>(() =>
                Trace.Initialize(new StaticConfigurationProvider(new Config.TracingConfig(isEnabled, string.Empty, string.Empty))));
        }

        [Test]
        public void CreateContext_should_initialize_if_needed_with_disabled_config()
        {
            Trace.Stop();
            using (var context = Trace.CreateRootContext("Test"))
                context.Should().BeSameAs(NoOpTraceContext.Instance);
        }

        [Test]
        public void TraceContext_Current_should_be_preserved_after_stop()
        {
            var traceContext = Trace.CreateRootContext("Test");
            Trace.Stop();
            TraceContext.Current.Should().BeSameAs(traceContext);
        }

        [Test]
        public void CreateRootContext_should_be_empty_after_stop()
        {
            Trace.Stop();
            Trace.CreateRootContext("Test").Should().BeSameAs(NoOpTraceContext.Instance);
        }
    }
}