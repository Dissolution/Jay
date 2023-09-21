using Jay.Memory;
using Jay.Text;
using Jay.Text.Extensions;

namespace Jay.Geometry;

public static partial class PointHelper
{
    public static string ToString<T>(T x, T y, ReadOnlySpan<char> format, IFormatProvider? _ = default)
    {
        var text = new InterpolatedText(3, 2);
        text.Write('(');
        text.Write<T>(x, format);
        text.Write(',');
        text.Write<T>(y, format);
        text.Write(')');
        return text.ToStringAndDispose();
    }
    
    public static string ToString<T>(T x, T y, string? format, IFormatProvider? _ = default)
    {
        var text = new InterpolatedText(3, 2);
        text.Write('(');
        text.Write<T>(x, format);
        text.Write(',');
        text.Write<T>(y, format);
        text.Write(')');
        return text.ToStringAndDispose();
    }

    public static string ToString<T>(T x, T y)
    {
        return $"({x},{y})";
    }
}