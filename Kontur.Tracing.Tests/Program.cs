using NUnitLite;
using System;
using NUnit.Common;
using System.Reflection;

public class Program
{
    public int Main(string[] args)
    {
#if DNX451
            return new AutoRun().Execute(args);
#else
        return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
#endif
    }
}