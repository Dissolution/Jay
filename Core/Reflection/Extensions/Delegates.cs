namespace Jay.Reflection.Extensions;

public delegate TValue Getter<TInstance, out TValue>(ref TInstance instance);
public delegate TValue StaticGetter<out TValue>();
public delegate TValue StructGetter<TStruct, out TValue>(ref TStruct instance) where TStruct : struct;
public delegate TValue ClassGetter<in TClass, out TValue>(TClass instance) where TClass : class;

public delegate void Setter<TInstance, in TValue>(ref TInstance instance, TValue value);
public delegate void StaticSetter<in TValue>(TValue value);
public delegate void StructSetter<TStruct, in TValue>(ref TStruct instance, TValue value) where TStruct : struct;
public delegate void ClassSetter<in TClass, in TValue>(TClass instance, TValue value) where TClass : class;

public delegate void EventAdder<TInstance, in THandler>(ref TInstance instance, THandler handler) where THandler : Delegate;
public delegate void EventRemover<TInstance, in THandler>(ref TInstance instance, THandler handler) where THandler : Delegate;
public delegate void EventRaiser<TInstance, in TEventArgs>(ref TInstance instance, TEventArgs args) where TEventArgs : EventArgs;
public delegate void EventDisposer<TInstance>(ref TInstance instance);

public delegate TInstance Constructor<out TInstance>(params object?[] args);
public delegate TReturn Invoker<TInstance, out TReturn>(ref TInstance instance, params object?[] args);

// public delegate TResult StaticInvoke<out TResult>(params object?[] args);
//
// public delegate TResult StructInvoke<TStruct, out TResult>(ref TStruct instance, params object?[] args)
//     where TStruct : struct;
//
// public delegate TResult ClassInvoke<TClass, out TResult>(TClass instance, params object[] args)
//     where TClass : class;
//
