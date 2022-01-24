/*using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay;
using Jay.Reflection;

public struct Box
{
    private object? _value;

    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value is null;
    }

    public Type? ValueType
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value?.GetType();
    }

    public Box()
    {
        _value = null;
    }

    public Box(object? value)
    {
        _value = value;
    }

    public bool Is<T>()
    {
        return _value is T;
    }

    public bool Is(Type? type)
    {
        return TypeExtensions.Implements(_value?.GetType(), type);
    }

    public bool Is<T>([NotNullWhen(true)] out T? value)
    {
        return _value.Is<T>(out value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, _value)) return true;
        if (obj is null) return false;
        if 
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}*/