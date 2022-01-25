using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay.Benchmarking;
using Jay.Collections.Pools;

Debug.Assert(args.Length == 0);

Label lblThing;

string name = Local.Thing(out lblThing);

Debugger.Break();


var result = Runner.RunAndOpenHtml();
Console.WriteLine(result);


var sbPool = Pool.Create<StringBuilder>(clean: sb => sb.Clear());

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

public class Local
{
    public static string Thing(out Label lbl, [CallerArgumentExpression("lbl")] string? labelName = null)
    {
        lbl = default;
        return labelName!;
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