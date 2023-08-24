using System.Text;
using Jay.Collections.Pooling;

namespace Jay.Text;

/// <summary>
/// A pool of <see cref="StringBuilder" /> instances that can be reused
/// </summary>
public sealed class StringBuilderPool : ObjectPool<StringBuilder>
{
    public static StringBuilderPool Shared { get; } = new();

    public StringBuilderPool()
        : base(
            factory: static () => new(),
            clean: static builder => builder.Clear())
    {
    }

    /// <summary>
    /// Returns the <see cref="StringBuilder"/> instance to this pool and then
    /// returns the <see cref="string"/> it built.
    /// </summary>
    public string ReturnToString(StringBuilder builder)
    {
        var str = builder.ToString();
        Return(builder);
        return str;
    }
    
    public new string Borrow(Action<StringBuilder> instanceAction)
    {
        StringBuilder sb = Rent();
        instanceAction.Invoke(sb);
        var str = sb.ToString();
        Return(sb);
        return str;
    }
}