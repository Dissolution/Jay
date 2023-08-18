#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay;

partial struct Option<T>
#if NET7_0_OR_GREATER
    : IEqualityOperators<Option<T>, Option<T>, bool>,
        IEqualityOperators<Option<T>, T, bool>
    //,IEqualityOperators<Option<T>, None, bool>
    //,IEqualityOperators<Option<T>, object, bool>
#endif
{
    public static implicit operator Option<T>(None none)
    {
        return None;
    }
    public static implicit operator Option<T>(T? value)
    {
        return Create(value);
    }

    public static bool operator ==(Option<T> option, Option<T> right)
    {
        return option.Equals(right);
    }
    public static bool operator !=(Option<T> option, Option<T> right)
    {
        return !option.Equals(right);
    }

    public static bool operator ==(Option<T> option, T? value)
    {
        return option.Equals(value);
    }
    public static bool operator !=(Option<T> option, T? value)
    {
        return !option.Equals(value);
    }
    public static bool operator ==(T? value, Option<T> option)
    {
        return option.Equals(value);
    }
    public static bool operator !=(T? value, Option<T> option)
    {
        return !option.Equals(value);
    }

    public static bool operator ==(Option<T> option, None none)
    {
        return option.Equals(none);
    }
    public static bool operator !=(Option<T> option, None none)
    {
        return !option.Equals(none);
    }
    public static bool operator ==(None none, Option<T> option)
    {
        return option.Equals(none);
    }
    public static bool operator !=(None none, Option<T> option)
    {
        return !option.Equals(none);
    }

    public static bool operator ==(Option<T> option, object? obj)
    {
        return option.Equals(obj);
    }
    public static bool operator !=(Option<T> option, object? obj)
    {
        return !option.Equals(obj);
    }
    public static bool operator ==(object? obj, Option<T> option)
    {
        return option.Equals(obj);
    }
    public static bool operator !=(object? obj, Option<T> option)
    {
        return !option.Equals(obj);
    }

    /// <summary>
    /// Create a new <see cref="Option{T}" /> based upon <paramref name="value" /><br />
    /// If <paramref name="value" /> is <c>null</c>, <see cref="None" /> will be returned,<br />
    /// otherwise, <see cref="Some" /> will be
    /// </summary>
    /// <param name="value">The value to base the returned <see cref="Option{T}" /> upon</param>
    /// <returns>An <see cref="Option{T}" /> indicating whether <paramref name="value" /> was <c>null</c></returns>
    public static Option<T> Create(T? value)
    {
        if (value.IsNone()) return None;
        return new(true, value);
    }

    /// <summary>
    /// Create an <see cref="Option{T}" /> containing a non-<c>null</c> <paramref name="value" />
    /// </summary>
    /// <param name="value">The non-<c>null</c> value to store</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value" /> is <c>null</c>, <see cref="Jay.None" />, or <see cref="DBNull" />
    /// </exception>
    public static Option<T> Some([NotNull] T value)
    {
        if (value.IsNone())
            throw new ArgumentNullException(nameof(value));
        return new(true, value);
    }

    /// <summary>
    /// Represents no value, the same as <see cref="Jay.None" />
    /// </summary>
    public static readonly Option<T> None = default;
}