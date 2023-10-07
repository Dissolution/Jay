using System.Text;
using Jay.Collections.Pooling;

namespace Jay.Text;

/// <summary>
/// A pool of <see cref="StringBuilder"/> instances that can be reused
/// </summary>
public static class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> _pool;

    static StringBuilderPool()
    {
        _pool = new ObjectPool<StringBuilder>(
            factory: static () => new StringBuilder(),
            clean: static builder => builder.Clear(),
            dispose: null);
    }

    public static StringBuilder Rent() => _pool.Rent();
    
    public static void Return(StringBuilder? builder) => _pool.Return(builder);

    public static string Borrow(Action<StringBuilder> build)
    {
        var builder = _pool.Rent();
        build(builder);
        string str = builder.ToString();
        _pool.Return(builder);
        return str;
    }
}