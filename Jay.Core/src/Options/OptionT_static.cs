#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay;

partial struct Option<T>
#if NET7_0_OR_GREATER
    : IEqualityOperators<Option<T>, Option<T>, bool>, 
      IEqualityOperators<Option<T>, T, bool>
#endif
{
    public static implicit operator Option<T>(T value) => Some(value);

    public static bool operator ==(Option<T> option, Option<T> right) => option.Equals(right);
    public static bool operator !=(Option<T> option, Option<T> right) => !option.Equals(right);

    public static bool operator ==(Option<T> option, T? value) => option.Equals(value);
    public static bool operator !=(Option<T> option, T? value) => !option.Equals(value);
    public static bool operator ==(T? value, Option<T> option) => option.Equals(value);
    public static bool operator !=(T? value, Option<T> option) => !option.Equals(value);

    public static bool operator ==(Option<T> option, object? obj) => option.Equals(obj);
    public static bool operator !=(Option<T> option, object? obj) => !option.Equals(obj);
    public static bool operator ==(object? obj, Option<T> option) => option.Equals(obj);
    public static bool operator !=(object? obj, Option<T> option) => !option.Equals(obj);
    
    /// <summary>
    /// Create an <see cref="Option{T}" /> containing <paramref name="value" />
    /// </summary>
    public static Option<T> Some(T value) => new(true, value);

    /// <summary>
    /// Represents no value, the same as <see cref="Void" />
    /// </summary>
    public static readonly Option<T> None = default;
}