using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay;
using Jay.Benchmarking;
using Jay.Collections.Pools;
using Jay.Reflection.Building;
using Jay.Text;

using var text = new TextBuilder();

var dm = RuntimeBuilder.CreateDynamicMethod<Func<string>>("anything");

object thing = 13;
ref int i = ref ObjectExtensions.UnboxRef<int>(thing);

i = 55;


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

public interface IEntity : INotifyPropertyChanged
{

}

public interface IEntity<TKey> : IEntity
{
    [Key]
    TKey Key { get; }
}

public interface INUEntity<TKey> : IEntity<TKey>
{
    string Name { get; }
    DateTime Updated { get; set; }
}

//RuntimeBuilder.Create<INUEntity<int>>();

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