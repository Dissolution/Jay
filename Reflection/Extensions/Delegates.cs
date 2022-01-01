namespace Jay.Reflection;

/// <summary>
/// Represents a placeholder <see cref="Type"/> for accessing <see langword="static"/> methods
/// </summary>
public struct Static
{
    private static Static _instance = default;

    /// <summary>
    /// Gets a <see langword="ref"/> to an instance of <see cref="Static"/> for use in accessing <see langword="static"/> methods
    /// </summary>
    public static ref Static Instance => ref _instance;
}

public delegate TValue? StaticGetter<out TValue>();

public delegate TValue? StructGetter<TStruct, out TValue>(ref TStruct instance)
    where TStruct : struct;

public delegate TValue? ObjectGetter<out TValue>(object? instance);

public delegate TValue? ClassGetter<in TClass, out TValue>(TClass? instance)
    where TClass : class;

public delegate void StaticSetter<in TValue>(TValue? value);

public delegate void StructSetter<TStruct, in TValue>(ref TStruct instance, TValue? value)
    where TStruct : struct;

public delegate void ObjectSetter<in TValue>(object? instance, TValue? value);

public delegate void ClassSetter<in TClass, in TValue>(TClass? instance, TValue? value)
    where TClass : class;


// TODO: Event delegates

public delegate TInstance Constructor<out TInstance>(params object?[] args);
//
// public delegate void Action<TInstance>(ref TInstance? instance, params object?[] args);
//
// public delegate TResult? Func<TInstance, out TResult>(ref TInstance? instance, params object?[] args);