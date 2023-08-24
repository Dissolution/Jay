using System.Runtime.CompilerServices;

namespace Jay.Tests;

public class NoneTests
{
    public readonly None None = default(None);
    public readonly object? NullObj = null;
    public readonly Nullable<int> NullInt = null;
    public readonly Option<object?> OptionNone = Option<object?>.None;
    
    [Fact]
    public void EqualsNull()
    {
        None none = default;
        
        Assert.True(none == None);
        Assert.True(none == NullObj);
        Assert.True(none == NullInt);
        Assert.True(none == OptionNone);
        
        Assert.True(None == none);
        Assert.True(NullObj == none);
        Assert.True(NullInt == none);
        Assert.True(OptionNone == none);
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