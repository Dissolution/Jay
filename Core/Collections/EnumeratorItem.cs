namespace Jay.Collections;

public sealed class EnumeratorItem<T> : IEquatable<T>
{
    public static implicit operator T?(EnumeratorItem<T?> enumeratorItem) 
        => enumeratorItem.Value;
    public static implicit operator (int Index, T? Value)(EnumeratorItem<T?> enumeratorItem)
        => (enumeratorItem.Index, enumeratorItem.Value);

    public readonly int Index;
    public readonly int? SourceLength;
    public readonly bool IsFirst;
    public readonly bool IsLast;
    public readonly T? Value;

    public EnumeratorItem(int index, int? sourceLength, bool isFirst, bool isLast, T? value)
    {
        Index = index;
        SourceLength = sourceLength;
        IsFirst = isFirst;
        IsLast = isLast;
        Value = value;
    }

    public void Deconstruct(out T? value)
    {
        value = Value;
    }

    public void Deconstruct(out int index, out T? value)
    {
        index = Index;
        value = Value;
    }

    public bool Equals(T? value)
    {
        return EqualityComparer<T>.Default.Equals(Value, value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is T value)
            return EqualityComparer<T>.Default.Equals(Value, value);
        return false;
    }

    public override int GetHashCode()
    {
        if (Value is null) return 0;
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        var text = new DefaultInterpolatedStringHandler();
        // Index
        text.AppendFormatted('[');
        text.AppendFormatted(Index);
        text.AppendFormatted('/');
        if (SourceLength.HasValue)
            text.AppendFormatted(SourceLength.Value);
        else
            text.AppendFormatted('?');
        text.AppendFormatted("] ");
        text.AppendFormatted(Value);
        return text.ToStringAndClear();
    }
}