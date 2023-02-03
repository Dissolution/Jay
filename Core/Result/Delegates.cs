namespace Jay.Result;

/// <summary>
/// A function that returns a <see cref="Result"/> and an <c>out</c> <paramref name="value"/>, similar to a <c>TryParse</c> operation
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/> output.</typeparam>
/// <returns>A <see cref="Result"/> for the operation</returns>
public delegate Result ResultOutFunc<T>(out T value);
