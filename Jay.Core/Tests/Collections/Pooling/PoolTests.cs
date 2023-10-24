using System.Text;
using Jay.Collections;

namespace Jay.Tests.Collections.Pooling;

public class PoolTests
{
    [Fact]
    public void CanCreateStringBuilder()
    {
        using var pool = ObjectPool.Create<StringBuilder>(factory: () => new StringBuilder());
        var builder = pool.Rent();
        Assert.NotNull(builder);
        Assert.Equal(0, builder.Length);
        pool.Return(builder);
    }

    [Fact]
    public void CanReuseStringBuilder()
    {
        using var pool = ObjectPool.Create<StringBuilder>(factory: () => new StringBuilder());

        var builder = pool.Rent();
        Assert.NotNull(builder);
        Assert.Equal(0, builder.Length);
        builder.Append("ABC");
        Assert.Equal(3, builder.Length);
        pool.Return(builder);

        var builder2 = pool.Rent();
        Assert.NotNull(builder2);
        Assert.Equal(3, builder2.Length);
        Assert.Equal("ABC", builder2.ToString());
        builder2.Append("DEF");
        Assert.Equal(6, builder2.Length);
        Assert.Equal("ABCDEF", builder2.ToString());
        pool.Return(builder2);

        var builder3 = pool.Rent();
        Assert.NotNull(builder3);
        Assert.Equal(6, builder3.Length);
        Assert.Equal("ABCDEF", builder3.ToString());
        pool.Return(builder3);
    }

    [Fact]
    public void CanCleanStringBuilder()
    {
        using var pool = ObjectPool.Create<StringBuilder>(
            factory: () => new StringBuilder(),
            clean: sb => sb.Clear(),
            dispose: null);

        var builder = pool.Rent();
        Assert.Equal(0, builder.Length);
        Assert.Equal("", builder.ToString());
        builder.Append("ABC");
        Assert.Equal(3, builder.Length);
        Assert.Equal("ABC", builder.ToString());
        pool.Return(builder);

        var builder2 = pool.Rent();
        Assert.Equal(0, builder2.Length);
        Assert.Equal("", builder2.ToString());
        builder2.Append("ABC");
        Assert.Equal(3, builder2.Length);
        Assert.Equal("ABC", builder2.ToString());
        pool.Return(builder2);

        var builder3 = pool.Rent();
        Assert.Equal(0, builder2.Length);
        Assert.Equal("", builder3.ToString());
        pool.Return(builder3);
    }

    [Fact]
    public void CanCleanArray()
    {
        using var pool = ObjectPool.Create<int[]>(
            factory: () => new int[8],
            clean: arr => arr.Initialize(0),
            dispose: null);

        var array = pool.Rent();
        Assert.Equal(8, array.Length);
        // ReSharper disable once RedundantAssignment
        array.AsSpan().ForEach((ref int item) => item = 3);
        Assert.True(array.All(item => item == 3));
        pool.Return(array);
        Assert.True(array.All(item => item == 0));
    }

    [Fact]
    public async Task IsAsyncSafe()
    {
        var rand = new Random();
        using var pool = ObjectPool.Create<StringBuilder>(() => new StringBuilder(), sb => sb.Clear());
        const int count = 100;
        var tasks = new Task<string>[count];
        for (var i = 0; i < count; i++)
        {
            tasks[i] = Task.Run<string>(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(rand.Next(0, 100)));
                    var sb = pool.Rent();
                    Assert.NotNull(sb);
                    Assert.Equal(0, sb.Length);
                    Assert.Equal("", sb.ToString());
                    sb.Append(Guid.NewGuid());
                    Assert.True(sb.Length > 0);
                    string? str = sb.ToString();
                    Assert.NotNull(str);
                    pool.Return(sb);
                    return str;
                });
        }
        var results = await Task.WhenAll(tasks);
        Assert.True(results.All(str => !string.IsNullOrWhiteSpace(str)));
        Assert.Equal(count, results.Distinct().Count());
    }
}