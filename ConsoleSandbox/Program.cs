using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Jay.Benchmarking;
using Jay.Collections.Pools;
using Jay.Corvidae;
using Jay.Text;

Debug.Assert(args.Length == 0);

//var result = Runner.RunAndOpenHtml();
//Console.WriteLine(result);

var thing = new Thing() {Id = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue)};

var local = new Local();
local.Log(LogLevel.Warning, $"We had an error with {thing:n=Thinger} at {DateTime.Now}");



Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;

public class Thing
{
    private int _id;

    public int Id
    {
        get => _id;
        set => _id = value;
    }
}

internal class Local : ILogger
{
    public bool IsEnabled(LogLevel level)
    {
        return true;
    }

    public void Log(ILogEvent logEvent)
    {
        throw new NotImplementedException();
    }

    public void Log(LogLevel level, [InterpolatedStringHandlerArgument("", "level")] LogInterpolatedStringHandler message)
    {
        if (IsEnabled(level))
        {
            using var text = new TextBuilder();
            message.Render(text);
            var str = text.ToString();
            Debugger.Break();
        }
        Debugger.Break();
    }

    public void Log(Exception? exception, LogLevel level, LogInterpolatedStringHandler message)
    {
        throw new NotImplementedException();
    }
}



 internal class Scope
 {
     public static string GetType(object? obj)
     {
         if (obj is null)
             return "null";
         if (obj is int i)
             return "int";

         return obj.GetType().Name;
     }

     public static string GetGenType<T>(T value) => typeof(T).Name;

     public static string GetGenValueType<T>(T value)
     {
         if (value is null) return "null";
         if (value is int i) return "int";
         return value.GetType().Name;
     }
 }