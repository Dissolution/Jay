﻿namespace Jay.Result;

/// <summary>
/// A function that returns a <see cref="Result"/> and an <c>out</c> <typeparamref name="T"/> <paramref name="value"/>, similar to a <c>TryParse</c> operation
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/> output.</typeparam>
/// <param name="value">The <c>out</c> <typeparamref name="T"/> value returned with the <see cref="Result"/></param>
/// <returns>A <see cref="Result"/> for the operation</returns>
public delegate Result ResultOutFunc<T>(out T value);
