namespace Jay.Reflection.Adapters;

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