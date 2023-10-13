namespace Jay.Reflection.Adapters;

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
    
/// <summary>
/// A generic delegate to get a <typeparamref name="TValue"/> from an <paramref name="instance"/>,
/// possibly through a <c>field</c> or <c>property</c>
/// </summary>
/// <typeparam name="TInstance">
/// The <see cref="Type"/> of the <paramref name="instance"/> the <typeparamref name="TValue"/> will be acquired from<br/>
/// If the <see cref="Type"/> is <c>static</c>, use <see cref="NoInstance"/>  
/// </typeparam>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of the value that will be returned
/// </typeparam>
/// <param name="instance">
/// A <c>ref</c> to the instance the value will be acquired from<br/>
/// If the <see cref="Type"/> is <c>static</c>, use <c>ref</c> <see cref="NoInstance.Ref">NoInstance.Ref</see>
/// </param>
/// <returns>
/// The value acquired from <paramref name="instance"/>
/// </returns>
public delegate TValue GetValue<TInstance, out TValue>(
    [Instance] ref TInstance instance);
    
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

public delegate TReturn Invoke<TInstance, out TReturn>(
    [Instance] ref TInstance instance, 
    params object?[] args);