﻿global using System.Reflection;

using System.Runtime.InteropServices;

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

[StructLayout(LayoutKind.Explicit, Size = 0)]
public readonly struct VOID
{

}


public delegate TValue? StaticGetter<out TValue>();

public delegate TValue? StructGetter<TStruct, out TValue>(ref TStruct instance)
    where TStruct : struct;

public delegate TValue? ClassGetter<in TClass, out TValue>(TClass instance)
    where TClass : class;


public delegate void StaticSetter<in TValue>(TValue? value);

public delegate void StructSetter<TStruct, in TValue>(ref TStruct instance, TValue? value)
    where TStruct : struct;

public delegate void ClassSetter<in TClass, in TValue>(TClass instance, TValue? value)
    where TClass : class;


// TODO: Event delegates
public delegate void EventAdder<TInstance, in THandler>(ref TInstance instance, THandler handler)
    where THandler : Delegate;

public delegate void EventRemover<TInstance, in THandler>(ref TInstance instance, THandler handler)
    where THandler : Delegate;

public delegate void EventRaiser<TInstance, in TEventArgs>(ref TInstance instance, TEventArgs args)
    where TEventArgs : EventArgs;

public delegate void EventDisposer<TInstance>(ref TInstance instance);


public delegate TInstance Constructor<out TInstance>(params object?[] args);

public delegate TResult StaticInvoke<out TResult>(params object?[] args);

public delegate TResult StructInvoke<TStruct, out TResult>(ref TStruct instance, params object?[] args)
    where TStruct : struct;

public delegate TResult ClassInvoke<TClass, out TResult>(TClass instance, params object[] args)
    where TClass : class;

