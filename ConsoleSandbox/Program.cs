using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building.Deconstruction;
using Jay.Text;
using Jay.Validation;
using System.Text.Json;

#if RELEASE
    var result = Runner.RunAndOpenHtml();
    Console.WriteLine(result);
#else
    using var text = new TextBuilder();

    var notification = new Notification
    {
        Id = Guid.Empty,
        Name = "Test",
        NotificationType = NotificationType.Email | NotificationType.SMS,
    };
    //ArgumentValidation.ThrowIf(notification, n => n == null || n.Id == Guid.Empty || n.NotificationType == NotificationType.Email);
    
    using var memoryStream = new MemoryStream();
    await JsonSerializer.SerializeAsync(memoryStream, notification);
    memoryStream.Seek(0L, SeekOrigin.Begin);
    using var streamReader = new StreamReader(memoryStream);
    var str = await streamReader.ReadToEndAsync();



    Debugger.Break();
    Console.WriteLine(text.ToString());
#endif
Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;

public static class Extensions
{
    public static string GetEvent<T, THandler>(this T? instance, Func<T?, THandler> eventInteraction)
        where THandler : Delegate
    {
        throw new NotImplementedException();
    }
}

public class Notification
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public NotificationType NotificationType { get; set; }
}

[Flags]
public enum NotificationType
{
    None = 0,
    Email = 1 << 0,
    SMS = 1 << 1,
}

public static class EventCapture<T> where T : class
{
    public static string GetEvent<THandler>(Action<T, THandler> captureEvent)
        where THandler : Delegate
    {
        var instructions = new RuntimeDeconstructor(captureEvent.Method).GetInstructions();
        
        Debugger.Break();
        return "";


        // var type = typeof(T);
        // var interfaces = type.GetInterfaces();
        // var iface = interfaces.First();
        // var obj = typeof(EventCaptureProxy<>).MakeGenericType(iface)
        //                                      .GetMethod("Decorate", Reflect.StaticFlags, new Type[1]{iface}).ThrowIfNull()
        //                                      .Invoke(null, new object?[1]{null});
        // T instance = obj as T;
        //
        // Debugger.Break();
        //
        //
        // captureEvent(instance);
        //
        // Debugger.Break();
        // return "";
    }
}

public class EventCaptureProxy<T> : DispatchProxy
    where T: class
{
    private readonly List<string> _eventNames;

    internal T? Instance { get; private set; }
    
    public EventCaptureProxy()
    {
        _eventNames = new List<string>(1);
    }

    /// <inheritdoc />
    protected override object? Invoke(MethodInfo? targetMethod, params object?[]? args)
    {
        Debugger.Break();
        return null;
    }
    
    public static T Decorate(T? instance = default)
    {
        // DispatchProxy.Create creates proxy objects
        var proxy = (Create<T, EventCaptureProxy<T>>() as EventCaptureProxy<T>)!;

        // If the proxy wraps an underlying object, it must be supplied after creating
        // the proxy.
        proxy.Instance = instance ?? Activator.CreateInstance<T>();

        return (proxy as T)!;
    }
}


public class Local
{
    // public static void Capture<T, THandler>(T thing, Action<T, THandler> func)
    //     where THandler : Delegate
    // {
    //
    // }
    
    public static void Capture<T, THandler>(T thing, Expression<Action<T, THandler>> func)
        where THandler : Delegate
    {

    }

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

public class Thing : INotifyPropertyChanged
{
    private int _id;

    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        throw new NotImplementedException();

        return base.TryBinaryOperation(binder, arg, out result);
    }

    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
    {
        throw new NotImplementedException();

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