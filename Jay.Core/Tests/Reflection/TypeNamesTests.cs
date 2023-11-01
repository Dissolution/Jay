using Jay.Reflection;

namespace Jay.Tests.Reflection;

public class TypeNamesTests
{
    [Fact]
    public void CheckNull()
    {
        Type? nullType = null;
        Assert.Equal("null", nullType.NameOf());

        object? obj = (object?)null;
        Assert.Equal("null", (obj?.GetType()).NameOf());

        Type? defaultType = default;
        Assert.Equal("null", defaultType.NameOf());
    }

    [Fact]
    public void CheckSystemTypes()
    {
        var byteType = typeof(byte);
        Assert.Equal("byte", byteType.NameOf());

        var intType = typeof(int);
        Assert.Equal("int", intType.NameOf());

        var guidType = typeof(Guid);
        Assert.Equal("Guid", guidType.NameOf());

        var voidType = typeof(void);
        Assert.Equal("void", voidType.NameOf());

        var objectType = typeof(object);
        Assert.Equal("object", objectType.NameOf());
    }

    [Fact]
    public void CheckGenericTypes()
    {
        var listType = typeof(List<>);
        Assert.Equal("List<>", listType.NameOf());

        var intListType = typeof(List<int>);
        Assert.Equal("List<int>", intListType.NameOf());

        var stringArrayType = typeof(string[]);
        Assert.Equal("string[]", stringArrayType.NameOf());

        var emptyDictType = typeof(Dictionary<,>);
        Assert.Equal("Dictionary<,>", emptyDictType.NameOf());

        var dictType = typeof(Dictionary<int, string>);
        Assert.Equal("Dictionary<int,string>", dictType.NameOf());
    }
}