#if NET7_0_OR_GREATER
using System.Numerics;
#endif
using System.Runtime.InteropServices;

namespace Jay;

/// <summary>
/// <b>None</b><br />
/// Create with <c>default</c> or <c>default(None)</c><br />
/// The opposite of <see cref="M:Option{T}.Some" />,
/// <see cref="None" /> is <c>==</c> and <see cref="Equals" />
/// <see cref="None" /> and <c>null</c><br />
/// Similar to <see cref="void" />, <c>Nil</c>, or <c>()</c> in other languages<br />
/// </summary>
[StructLayout(LayoutKind.Auto, Size = 0)]
public readonly struct None :
    IEquatable<None>
#if NET7_0_OR_GREATER
    , IEqualityOperators<None, None, bool>
    , IEqualityOperators<None, object, bool>
#endif
{
    public static bool operator ==(None a, None z)
    {
        return true;
    }
    public static bool operator !=(None a, None z)
    {
        return false;
    }

    public static bool operator ==(None none, object? obj)
    {
        return obj.IsNone();
    }
    public static bool operator !=(None none, object? obj)
    {
        return !obj.IsNone();
    }
    public static bool operator ==(object? obj, None none)
    {
        return obj.IsNone();
    }
    public static bool operator !=(object? obj, None none)
    {
        return !obj.IsNone();
    }

    public bool Equals(None none)
    {
        return true;
    }
    public override bool Equals(object? obj)
    {
        return obj.IsNone();
    }
    public override int GetHashCode()
    {
        return 0;
    }
    public override string ToString()
    {
        return nameof(None);
    }
}