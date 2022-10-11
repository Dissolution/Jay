using System.Reflection;

namespace Jay.BenchTests;

public class CanBeNullTests
{
    [Fact]
    public void Null()
    {
        Assert.True(((Type?)null).CanContainNull());
    }

    [Fact]
    public void Nullable()
    {
        Assert.True(typeof(int?).CanContainNull());
        Assert.True(typeof(Nullable<int>).CanContainNull());
        Assert.True(typeof(Nullable<>).CanContainNull());
        Assert.True(typeof(Nullable<>).MakeGenericType<Guid>().CanContainNull());
    }

    [Fact]
    public void ValueTypes()
    {
        Assert.False(typeof(int).CanContainNull());
        Assert.False(typeof(BindingFlags).CanContainNull());
        Assert.False(typeof(Span<byte>).CanContainNull());
        Assert.False(typeof(ReadOnlySpan<>).CanContainNull());
    }

    [Fact]
    public void ClassTypes()
    {
        Assert.True(typeof(object).CanContainNull());
        Assert.True(typeof(string).CanContainNull());
        Assert.True(typeof(CanBeNullTests).CanContainNull());
        Assert.True(typeof(ICloneable).CanContainNull());
        Assert.True(typeof(ISpanFormattable).CanContainNull());
    }

    [Fact]
    public unsafe void Pointers()
    {
        Assert.True(typeof(void*).CanContainNull());
        Assert.True(typeof(char*).CanContainNull());
    }

    [Fact]
    public void Refs()
    {
        Assert.True(typeof(byte).MakeByRefType().CanContainNull());
        Assert.True(typeof(object).MakeByRefType().CanContainNull());
    }
}