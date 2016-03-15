using System;
using System.Diagnostics;

// ReSharper disable CheckNamespace
namespace JetBrains.Annotations
// ReSharper restore CheckNamespace
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.Delegate)]
    [Conditional("JETBRAINS_ANNOTATIONS")]
    internal sealed class CanBeNullAttribute : Attribute
    {
    }

    [Conditional("JETBRAINS_ANNOTATIONS")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.Delegate)]
    internal sealed class NotNullAttribute : Attribute
    {
    }
}