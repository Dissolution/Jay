namespace Jay.Reflection.Conversion;

public interface ICaster
{
    bool CanCastFrom(Type inType);
    bool CanCastTo(Type outType);

    Result TryCast(object? input, [NotNullWhen(true)] out object? output, CastOptions options = default);
}