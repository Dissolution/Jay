using Jay.Text;

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
        this.Index = index;
        this.SourceLength = sourceLength;
        this.IsFirst = isFirst;
        this.IsLast = isLast;
        this.Value = value;
    }

    public void Deconstruct(out T? value)
    {
        value = this.Value;
    }

    public void Deconstruct(out int index, out T? value)
    {
        index = this.Index;
        value = this.Value;
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
        using var text = TextBuilder.Borrow();
        // Index
        text.Append('[').Append(Index).Write('/');
        if (SourceLength.HasValue)
            text.Write(SourceLength.Value);
        else
            text.Write('?');
        text.Append("] ").Write(Value);
        return text.ToString();
    }
}