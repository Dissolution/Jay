using System.Text;
using Jay.Collections.Pooling;

namespace Jay.Text;

public sealed class StringBuilderPool : ObjectPool<StringBuilder>
{
    public static StringBuilderPool Shared { get; } = new();

    public StringBuilderPool()
        : base(
            factory: static () => new StringBuilder(), 
            clean: static builder => builder.Clear())
    { }

    public string ReturnToString(StringBuilder builder)
    {
        string str = builder.ToString();
        Return(builder);
        return str;
    }
    
    public new string Use(Action<StringBuilder> instanceAction)
    {
        Validate.NotNull(instanceAction);
        StringBuilder sb = Rent();
        instanceAction.Invoke(sb);
        string str = sb.ToString();
        Return(sb);
        return str;
    }
}