﻿namespace Jay.Reflection.Adapters;

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