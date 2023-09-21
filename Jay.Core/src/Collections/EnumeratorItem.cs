#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

using Jay.Utilities;

namespace Jay.Collections;

public sealed class EnumeratorItem<T> : IEquatable<T>
{
    public readonly int Index;
    public readonly bool IsFirst;
    public readonly bool IsLast;
    public readonly int? SourceLength;
    public readonly T Value;

    public EnumeratorItem(int index, int? sourceLength, bool isFirst, bool isLast, T value)
    {
        Index = index;
        SourceLength = sourceLength;
        IsFirst = isFirst;
        IsLast = isLast;
        Value = value;
    }

    public bool Equals(T? value)
    {
        return EqualityComparer<T>.Default.Equals(Value, value);
    }
    public static implicit operator T(EnumeratorItem<T> enumeratorItem)
    {
        return enumeratorItem.Value;
    }
    public static implicit operator (int Index, T Value)(EnumeratorItem<T> enumeratorItem)
    {
        return (enumeratorItem.Index, enumeratorItem.Value);
    }

    public void Deconstruct(out T value)
    {
        value = Value;
    }

    public void Deconstruct(out int index, out T value)
    {
        index = Index;
        value = Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is T value)
            return EqualityComparer<T>.Default.Equals(Value, value);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(Value);
    }

    public override string ToString()
    {
        return $"[{Index}/{SourceLength}] {Value}";
    }
}