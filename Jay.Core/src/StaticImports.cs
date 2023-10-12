namespace Jay;

// ReSharper disable once InconsistentNaming
public static class StaticImports
{
    public static Option<T> None<T>() => Option<T>.None;
    
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    
    
    public static Result Ok() 
        => Result.Ok(); 
    
    public static Result Error(Exception? error) 
        => Result.Error(error);
    
    
    public static Result<TValue> Ok<TValue>(TValue value) 
        => Result<TValue>.Ok(value);
    
    public static Result<TValue> Error<TValue>(Exception? error) 
        => Result<TValue>.Error(error);


    public static Result<V, E> Ok<V, E>(V okValue)
        => Result<V, E>.Ok(okValue);

    public static Result<V, E> Error<V, E>(E? errorValue)
        => Result<V, E>.Error(errorValue);
    
    
     /// <summary>
    /// Tries to invoke the <paramref name="action"/> and returns a <see cref="Result"/>
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to <see cref="Action.Invoke"/></param>
    /// <returns>
    /// A successful <see cref="Result"/> if the <paramref name="action"/> invokes without throwing an <see cref="Exception"/>.
    /// A failed <see cref="Result"/> with the caught <see cref="Exception"/> attached if not.
    /// </returns>
    public static Result TryInvoke(
        [AllowNull, NotNullWhen(true)] 
        this Action? action)
    {
        if (action is null)
        {
            return new ArgumentNullException(nameof(action));
        }
        try
        {
            action.Invoke();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Error(ex);
        }
    }

    /// <summary>
    /// Tries to invoke the <paramref name="func"/>, setting <paramref name="value"/> and returning a <see cref="Result"/>
    /// </summary>
    /// <typeparam name="TValue">The <see cref="Type"/> of <paramref name="value"/> the <paramref name="func"/> produces</typeparam>
    /// <param name="func">The <see cref="Func{T}"/> to <see cref="Func{T}.Invoke"/></param>
    /// <param name="value">The result of invoking <paramref name="func"/> or <see langword="default{TResult}"/> on failure.</param>
    /// <returns>
    /// A successful <see cref="Result"/> if the <paramref name="func"/> invokes without throwing an <see cref="Exception"/>.
    /// A failed <see cref="Result"/> with the caught <see cref="Exception"/> attached if not.
    /// </returns>
    public static Result TryInvoke<TValue>(
        [AllowNull, NotNullWhen(true)] 
        this Func<TValue>? func,
        [MaybeNullWhen(false)] 
        out TValue value)
    {
        if (func is null)
        {
            value = default;
            return new ArgumentNullException(nameof(func));
        }
        try
        {
            value = func.Invoke();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            value = default;
            return Result.Error(ex);
        }
    }

    /// <summary>
    /// Try to execute the given <paramref name="func"/> and return a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="func">The function to try to execute.</param>
    /// <returns>
    /// A passing <see cref="Result{T}"/> containing <paramref name="func"/>'s return value or
    /// a failing <see cref="Result{T}"/> containing the captured <see cref="Exception"/>.
    /// </returns>
    public static Result<TValue> TryInvoke<TValue>(
        [AllowNull,NotNullWhen(true)] 
        this Func<TValue>? func)
    {
        if (func is null)
        {
            return new ArgumentNullException(nameof(func));
        }
        try
        {
            TValue value = func.Invoke();
            return Result<TValue>.Ok(value);
        }
        catch (Exception ex)
        {
            return Result<TValue>.Error(ex);
        }
    }

    /// <summary>
    /// Invokes a <paramref name="func"/> and returns its result <br/>
    /// If the function throws an <see cref="Exception"/>, a <paramref name="fallback"/> value will be returned instead
    /// </summary>
    /// <param name="func">The <see cref="Func{TResult}"/> to invoke</param>
    /// <param name="fallback">The value to return if <paramref name="func"/> throws an <see cref="Exception"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to be returned</typeparam>
    /// <returns>The result of <paramref name="func"/> or <paramref name="fallback"/></returns>
    [return: NotNullIfNotNull(nameof(fallback))]
    public static T? InvokeOrDefault<T>(
        this Func<T>? func,
        T? fallback = default)
    {
        if (func is null)
        {
            return fallback;
        }
        try
        {
            return func();
        }
        catch
        {
            return fallback;
        }
    }
}