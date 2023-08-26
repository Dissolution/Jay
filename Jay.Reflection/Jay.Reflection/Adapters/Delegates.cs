namespace Jay.Reflection.Adapters;

/// <summary>
/// A generic delegate to set the <paramref name="value"/> of an <paramref name="instance"/>,
/// possibly through a <c>field</c> or <c>property</c>
/// </summary>
/// <typeparam name="TInstance">
/// The <see cref="Type"/> of the instance the <paramref name="value"/> will be set upon<br/>
/// If the <see cref="Type"/> is <c>static</c>, use <see cref="NoInstance"/> instead  
/// </typeparam>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of the <paramref name="value"/> that will be set
/// </typeparam>
/// <param name="instance">
/// A <c>ref</c> to the instance the <paramref name="value"/> will be set upon<br/>
/// If the <see cref="Type"/> is <c>static</c>, use <see cref="NoInstance.Ref"/> instead
/// </param>
/// <param name="value">
/// The value that will be set in <paramref name="instance"/>
/// </param>
public delegate void SetValue<TInstance, in TValue>(
    [Instance] ref TInstance instance, 
    TValue value);

/// <summary>
/// A generic delegate to get the value of an <paramref name="instance"/>,
/// possibly through a <c>field</c> or <c>property</c>
/// </summary>
/// <typeparam name="TInstance">
/// The <see cref="Type"/> of the instance the <paramref name="value"/> will be acquired from<br/>
/// If the <see cref="Type"/> is <c>static</c>, use <see cref="NoInstance"/> instead  
/// </typeparam>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of the <paramref name="value"/> that will be returned
/// </typeparam>
/// <param name="instance">
/// A <c>ref</c> to the instance the <paramref name="value"/> will be acquired from<br/>
/// If the <see cref="Type"/> is <c>static</c>, use <see cref="NoInstance.Ref"/> instead
/// </param>
/// <returns>
/// The value acquired from <paramref name="instance"/>
/// </returns>
public delegate TValue GetValue<TInstance, out TValue>(
    [Instance] ref TInstance instance);


public delegate void AddHandler<TInstance, in THandler>(
    [Instance] ref TInstance instance, 
    THandler eventHandler)
    where THandler : Delegate;

public delegate void RemoveHandler<TInstance, in THandler>(
    [Instance] ref TInstance instance, 
    THandler eventHandler)
    where THandler : Delegate;

public delegate void RaiseHandler<TInstance>(
    [Instance] ref TInstance instance, 
    params object?[] eventArgs);

public delegate TInstance Construct<out TInstance>(
    params object?[] args);

public delegate TReturn Invoke<TInstance, out TReturn>(
    [Instance] ref TInstance instance, 
    params object?[] args);
