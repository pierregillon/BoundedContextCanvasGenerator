using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BoundedContextCanvasGenerator.Tests.Integration.Utils;

public class ProgramRunner<T>
{
    public (string Output, string Error) Run(IEnumerable<string> args)
    {
        var outputStringBuilder = new StringBuilder();
        using var outputWriter = new StringWriter(outputStringBuilder);
        Console.SetOut(outputWriter);

        var errorStringBuilder = new StringBuilder();
        using var errorWriter = new StringWriter(outputStringBuilder);
        Console.SetError(errorWriter);

        Execute(args.ToArray());

        return (outputStringBuilder.ToString(), errorStringBuilder.ToString());
    }

    private static void Execute(IEnumerable args)
    {
        var programAssembly = Assembly.GetAssembly(typeof(T));
        if (programAssembly is null) {
            throw new InvalidOperationException("Unable to find the bounded context generator program");
        }

        programAssembly.EntryPoint?.Invoke(null, new object?[] { args });
    }
}