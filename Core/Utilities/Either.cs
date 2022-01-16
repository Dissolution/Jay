using System.Diagnostics.CodeAnalysis;

namespace Jay.Utilities;

public readonly struct Either<T1, T2>
{
    public static implicit operator Either<T1, T2>(T1 value) => new Either<T1, T2>(value);
    public static implicit operator Either<T1, T2>(T2 value) => new Either<T1, T2>(value);

    public static explicit operator T1(Either<T1, T2> either)
    {
        if (either.Is(out T1? value))
            return value;
        throw new InvalidOperationException();
    }
    public static explicit operator T2(Either<T1, T2> either)
    {
        if (either.Is(out T2? value))
            return value;
        throw new InvalidOperationException();
    }


    public static bool operator ==(Either<T1, T2> x, Either<T1, T2> y) => x.Equals(y);
    public static bool operator !=(Either<T1, T2> x, Either<T1, T2> y) => !x.Equals(y);
    public static bool operator ==(Either<T1, T2> x, T1? y) => x.Equals(y);
    public static bool operator !=(Either<T1, T2> x, T1? y) => !x.Equals(y);
    public static bool operator ==(Either<T1, T2> x, T2? y) => x.Equals(y);
    public static bool operator !=(Either<T1, T2> x, T2? y) => !x.Equals(y);

    private readonly T1? _value1;
    private readonly T2? _value2;
    private readonly int _index;

    public Either(T1 value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _index = 1;
        _value1 = value;
        _value2 = default;
    }

    public Either(T2 value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _index = 2;
        _value1 = default;
        _value2 = value;
    }

    public void Deconstruct(out T1? value)
    {
        value = _value1;
    }

    public void Deconstruct(out T2? value)
    {
        value = _value2;
    }

    public bool Is([NotNullWhen(true)] out T1? value)
    {
        if (_index == 1)
        {
            value = _value1!;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public bool Is([NotNullWhen(true)] out T2? value)
    {
        if (_index == 2)
        {
            value = _value2!;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public bool Equals(Either<T1, T2> either)
    {
        if (either._index == 1)
            return _index == 1 && EqualityComparer<T1>.Default.Equals(either._value1, _value1);
        if (either._index == 2)
            return _index == 2 && EqualityComparer<T2>.Default.Equals(either._value2, _value2);
        return false;
    }

    public bool Equals(T1? value)
    {
        return _index == 1 && EqualityComparer<T1>.Default.Equals(_value1, value);
    }

    public bool Equals(T2? value)
    {
        return _index == 2 && EqualityComparer<T2>.Default.Equals(_value2, value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is T1 value1) return Equals(value1);
        if (obj is T2 value2) return Equals(value2);
        if (obj is Either<T1, T2> either) return Equals(either);
        return false;
    }

    public override int GetHashCode()
    {
        return _index == 1 ? _value1!.GetHashCode() : _value2!.GetHashCode();
    }

    public override string ToString()
    {
        return (_index == 1 ? _value1!.ToString() : _value2!.ToString()) ?? "";
    }
}