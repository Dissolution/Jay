using System.Collections.Concurrent;

namespace Jay.Text;

public static class StringBuilderPool
{
    private static readonly ConcurrentStack<StringBuilder> _builderPool;

    static StringBuilderPool()
    {
        _builderPool = new(new[] { CreateStringBuilder() });
    }

    private static StringBuilder CreateStringBuilder() => new StringBuilder(1024);

    public static StringBuilder Rent()
    {
        if (_builderPool.TryPop(out var stringBuilder))
            return stringBuilder;
        return CreateStringBuilder();
    }

    public static void Return(this StringBuilder stringBuilder)
    {
        _builderPool.Push(stringBuilder);
    }
}