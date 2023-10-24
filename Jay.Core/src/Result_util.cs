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
            return new Result<TValue>(ok, value, default);
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


   
}