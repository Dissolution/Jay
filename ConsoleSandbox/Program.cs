using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay;
using Jay.Benchmarking;
using Jay.Collections;
using Jay.Collections.Pools;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Text;
using Jay.Validation;

using var text = new TextBuilder();
//
// object a = 13;
// dynamic A = new DynamicWrapper(a);
//
// object b = 147;
// dynamic B = new DynamicWrapper(b);
//
// var c = A + B;
// text.Write(c);


Expression<Func<bool>> a = () => true;
Expression<Func<bool>> b = () => false;
var c = 


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

public sealed class DynamicWrapper : DynamicObject
{
    private static readonly ConcurrentTypeDictionary<ConcurrentDictionary<ExpressionType, Delegate>> _cache;

    static DynamicWrapper()
    {
        _cache = new ConcurrentTypeDictionary<ConcurrentDictionary<ExpressionType, Delegate>>();
    }




    private readonly object? _obj;
    private readonly Type? _objType;

    public DynamicWrapper(object? obj)
    {
        _obj = obj;
        _objType = obj?.GetType();
    }

    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)
    {
        var op = binder.Operation;
        Expressions.CompileBinaryExpression<Func>()

        return base.TryBinaryOperation(binder, arg, out result);
    }

    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
    {
        return base.TryUnaryOperation(binder, out result);
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

    public static T GenericType<T>(T value)
    {
        return value;
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