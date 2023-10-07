using Jay.Utilities;

namespace Jay;

/* Static Utilities involving Result, Result<T>, and Result<V,E> */
partial struct Result
{
    public delegate bool Try<TValue>(out TValue value);
    public delegate Result TryResult<TValue>(out TValue value);

    public static Result<TValue> Wrap<TValue>(Try<TValue> tryFunc)
    {
        try
        {
            bool ok = tryFunc(out TValue value);
            if (ok)
                return value;
            return default;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public static Result<TValue> Wrap<TValue>(TryResult<TValue> tryFunc)
    {
        try
        {
            Result result = tryFunc(out TValue value);
            return new Result<TValue>(result._ok, value, result._exception);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result InvokeUntilError(params Action?[]? actions)
    {
        if (actions is null) return Ok();
        Result result;
        for (var i = 0; i < actions.Length; i++)
        {
            result = actions[i].TryInvoke();
            if (!result) return result;
        }
        return Ok();
    }

    public static Result InvokeUntilError(IEnumerable<Action?>? actions)
    {
        if (actions is null)
            return Ok();

        Result result;
        foreach (var action in actions)
        {
            result = action.TryInvoke();
            if (!result)
                return result;
        }
        return Ok();
    }

#region Dispose
    /// <summary>
    /// Tries to dispose of <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/> to dispose</typeparam>
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