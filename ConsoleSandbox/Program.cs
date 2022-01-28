using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay;
using Jay.Benchmarking;
using Jay.Collections.Pools;
using Jay.Reflection;
using Jay.Text;

using var text = new TextBuilder();


object? thing = 147;

var a = thing.Is(out int i);
var b = thing.Is(out decimal m);

int k = 0;
ref int reffy = ref k;
//var c = thing.Is(out reffy);
//var c = thing.TryUnboxRef(ref reffy);
//var c = thing.TryUnboxRef(ref int newIntRef);
//reffy = ref thing.UnboxRef<int>();

int origReffy = reffy;
reffy = 13;






//
// text.WriteAligned("abcd", Alignment.Left, 2, 'x');
// text.WriteAligned("abcd", Alignment.Right, 2, '-');
// text.WriteAligned("abcd", Alignment.Center | Alignment.Right, 2, '|');
//

string str = text.ToString();

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
    private static class S<T>
    {
        public static T Default = default;
    }

    public static ref T GetRef<T>()
    {
        return ref S<T>.Default;
    }

    public static void Me<T>(out T value)
    {
        value = GetRef<T>();
    }
  



    public static string CaptureOutParamName(out Label lbl, [CallerArgumentExpression("lbl")] string? labelName = null)
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