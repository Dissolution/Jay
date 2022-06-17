namespace Jay.Conversion;

public interface ICaster<TIn, TOut> : ICaster
{
    bool ICaster.CanCastFrom(Type inType) => inType.IsAssignableTo(typeof(TIn));
    bool ICaster.CanCastTo(Type outType) => outType.IsAssignableTo(typeof(TOut));

    Result.Result ICaster.TryCast(object? input, [NotNullWhen(true)] out object? output, CastOptions options)
    {
        if (input is TIn)
        {
            Result.Result result = TryCast((TIn)input, out TOut? outValue, options);
            if (!result)
            {
                output = null;
                return result;
            }
            else
            {
                output = outValue!;
                return true;
            }
        }
        else
        {
            output = null;
            return new CastException(input?.GetType(), typeof(TOut));
        }
    }

    Result.Result TryCast(TIn? input, [NotNullWhen(true)] out TOut? output, CastOptions options = default);
}