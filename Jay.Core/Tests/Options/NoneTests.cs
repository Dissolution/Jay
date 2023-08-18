using System.Runtime.CompilerServices;
using Jay;

namespace Jay.Tests.Options;

public class NoneTests
{
    public readonly None None = default(None);
    public readonly object? NullObj = null;
    public readonly DBNull DbNull = DBNull.Value;
    public readonly Nullable<int> NullInt = null;
    public readonly Option<object?> OptionNone = Option<object?>.None;
    public readonly Option<object?> OptionCreateNullObj = Option<object?>.Create(null);
    public readonly Option<int?> OptionCreateNullInt = Option<int?>.Create(null);
    
    [Fact]
    public void EqualsNull()
    {
        None none = default;
        
        Assert.True(none == None);
        Assert.True(none == NullObj);
        Assert.True(none == DbNull);
        Assert.True(none == NullInt);
        Assert.True(none == OptionNone);
        Assert.True(none == OptionCreateNullObj);
        Assert.True(none == OptionCreateNullInt);
        
        Assert.True(None == none);
        Assert.True(NullObj == none);
        Assert.True(DbNull == none);
        Assert.True(NullInt == none);
        Assert.True(OptionNone == none);
        Assert.True(OptionCreateNullObj == none);
        Assert.True(OptionCreateNullInt == none);
    }

    [Fact]
    public void NoneWorks()
    {
        //None none = default;
        int size = Unsafe.SizeOf<None>();
        Assert.Equal(1, size);

        var hashset = new Dictionary<string, None>();
        hashset.Add("abc", default);
        hashset.Add("abcde", default);
        Assert.Equal(2, hashset.Count);
        Assert.True(hashset.ContainsKey("abc"));
        Assert.False(hashset.ContainsKey("abcd"));
        Assert.True(hashset.ContainsValue(default));
        hashset.Clear();
        Assert.False(hashset.ContainsValue(default));
    }
}