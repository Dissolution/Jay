namespace Jay;

partial struct Result
{
    /// <summary>
    /// Tries to invoke the <paramref name="action" /> and returns a <see cref="Result" />
    /// </summary>
    /// <param name="action">The <see cref="Action" /> to <see cref="Action.Invoke" /></param>
    /// <returns>
    /// A successful <see cref="Result" /> if the <paramref name="action" /> invokes without throwing an <see cref="Exception" />.
    /// A failed <see cref="Result" /> with the caught <see cref="Exception" /> attached if not.
    /// </returns>
    public static Result TryInvoke(
        [AllowNull, NotNullWhen(true)] 
        Action? action)
    {
        if (action is null)
        {
            return new ArgumentNullException(nameof(action));
        }
        try
        {
            action.Invoke();
            return Ok;
        }
        catch (Exception ex)
        {
            return Error(ex);
        }
    }

    /// <summary>
    /// Tries to invoke the <paramref name="func" />, setting <paramref name="output" /> and returning a <see cref="Result" />
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of <paramref name="output" /> the <paramref name="func" /> produces</typeparam>
    /// <param name="func">The <see cref="Func{T}" /> to <see cref="Func{T}.Invoke" /></param>
    /// <param name="output">The result of invoking <paramref name="func" /> or <see langword="default{TResult}" /> on failure.</param>
    /// <returns>
    /// A successful <see cref="Result" /> if the <paramref name="func" /> invokes without throwing an <see cref="Exception" />.
    /// A failed <see cref="Result" /> with the caught <see cref="Exception" /> attached if not.
    /// </returns>
    public static Result TryInvoke<T>(
        [AllowNull,NotNullWhen(true)] 
        Func<T>? func,
        [MaybeNullWhen(false)] 
        out T output)
    {
        if (func is null)
        {
            output = default;
            return new ArgumentNullException(nameof(func));
        }
        try
        {
            output = func.Invoke();
            return Ok;
        }
        catch (Exception ex)
        {
            output = default;
            return Error(ex);
        }
    }

    /// <summary>
    /// Try to execute the given <paramref name="func" /> and return a <see cref="Result{T}" />.
    /// </summary>
    /// <param name="func">The function to try to execute.</param>
    /// <returns>
    /// A passing <see cref="Result{T}" /> containing <paramref name="func" />'s return value or
    /// a failing <see cref="Result{T}" /> containing the captured <see cref="_error" />.
    /// </returns>
    public static Result<T> TryInvoke<T>(
        [AllowNull,NotNullWhen(true)] 
        Func<T>? func)
    {
        if (func is null)
        {
            return new ArgumentNullException(nameof(func));
        }
        try
        {
            return Result<T>.Ok(func.Invoke());
        }
        catch (Exception ex)
        {
            return Result<T>.Error(ex);
        }
    }

    /// <summary>
    /// Invokes the <paramref name="func" /> and returns its result.
    /// If the <paramref name="func" /> throws an <see cref="Exception" />, <paramref name="fallback" /> is returned instead.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of <paramref name="output" /> the <paramref name="func" /> produces</typeparam>
    /// <param name="func">The <see cref="Func{T}" /> to <see cref="Func{T}.Invoke" /></param>
    /// <param name="fallback">The value to return if invocation fails.</param>
    /// <returns><paramref name="func" />'s result or <paramref name="fallback" /></returns>
    [return: NotNullIfNotNull(nameof(fallback))]
    public static T? InvokeOrDefault<T>(Func<T>? func, T? fallback = default)
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

    public static Result InvokeUntilError(params Action?[]? actions)
    {
        if (actions is null) return Ok;
        Result result;
        for (var i = 0; i < actions.Length; i++)
        {
            result = TryInvoke(actions[i]);
            if (!result) return result;
        }
        return Ok;
    }

    public static Result InvokeUntilError(IEnumerable<Action?>? actions)
    {
        if (actions is null)
            return Ok;

        Result result;
        foreach (var action in actions)
        {
            result = TryInvoke(action);
            if (!result)
                return result;
        }
        return Ok;
    }

#region Dispose
    /// <summary>
    /// Tries to dispose of <paramref name="value" />.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of <paramref name="value" /> to dispose</typeparam>
    /// <param name="value">The value to dispose.</param>
    public static void Dispose<T>(T? value)
    {
        // Avoids boxing for disposable structs
        if (value is IDisposable)
        {
            try
            {
                ((IDisposable)value).Dispose();
            }
            catch // (Exception ex)
            {
                // Ignore all exceptions
            }
        }
    }

#if !(NET48 || NETSTANDARD2_0)
    public static async ValueTask DisposeAsync<T>(T? value)
    {
        // Avoids boxing for disposable structs
        if (value is IAsyncDisposable)
        {
            try
            {
                // Avoids boxing for disposable structs
                await ((IAsyncDisposable)value).DisposeAsync();
            }
            catch // (Exception ex)
            {
                // Ignore all exceptions
            }
        }
        // Avoids boxing for disposable structs
        else if (value is IDisposable)
        {
            try
            {
                // Avoids boxing for disposable structs
                ((IDisposable)value).Dispose();
            }
            catch // (Exception ex)
            {
                // Ignore all exceptions
            }
        }
    }
#endif
#endregion
}