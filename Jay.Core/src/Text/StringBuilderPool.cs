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
            static () => new(),
            static builder => builder.Clear())
    {
    }

    public string ReturnToString(StringBuilder builder)
    {
        var str = builder.ToString();
        Return(builder);
        return str;
    }

    public new string Use(Action<StringBuilder> instanceAction)
    {
        Validate.NotNull(instanceAction);
        StringBuilder sb = Rent();
        instanceAction.Invoke(sb);
        var str = sb.ToString();
        Return(sb);
        return str;
    }
}