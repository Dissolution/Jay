using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jay.Comparision;
using Jay.Randomization;
using Jay.Reflection.Cloning;

namespace Jay.Tests;

public class CloneTests
{
    [Fact]
    public void CloneNull()
    {
        object? obj = null;
        object? clone = Muq.Clone<object>(obj);
        Assert.True(clone is null);
        Assert.True(ReferenceEquals(obj, clone));
        Assert.Equal(obj, clone);
    }

    [Fact]
    public void CloneStringNull()
    {
        string? str = null;
        string? clone = Muq.Clone<string>(str);
        Assert.True(clone is null);
        Assert.True(string.Equals(str, clone));
        Assert.True(str == clone);
        Assert.Equal(str, clone);
    }

    [Fact]
    public void CloneNullableNull()
    {
        int? value = null;
        int? clone = Muq.Clone<int?>(value);
        Assert.True(clone == null);
        Assert.True(!clone.HasValue);
        Assert.Equal(value, clone);
    }

    [Fact]
    public void CloneString()
    {
        string str = Randomizer.BitString;
        string clone = Muq.Clone<string>(str);
        Assert.NotNull(clone);
        Assert.True(str == clone);
        Assert.Equal(str, clone);
    }

    [Fact]
    public void CloneInt32()
    {
        int value = Randomizer.Generate<int>();
        int clone = Muq.Clone<int>(value);
        Assert.True(value == clone);
        Assert.Equal(value, clone);
    }

    [Fact]
    public void CloneGuid()
    {
        Guid value = Randomizer.Generate<Guid>();
        Guid clone = Muq.Clone<Guid>(value);
        Assert.True(value == clone);
        Assert.Equal(value, clone);
    }

    [Fact]
    public void CloneNonRef()
    {
        var value = (DateTime.Now, 147);
        var clone = Muq.Clone(value);
        Assert.True(value == clone);
        Assert.Equal(value, clone);

        value.Now = DateTime.Now;
        value.Item2 = 33;
        Assert.False(value == clone);
        Assert.NotEqual(value, clone);
    }

    [Fact]
    public void CloneClass()
    {
        var value = new Tuple<int, string>(3, "Three");
        var clone = Muq.Clone(value);
        Assert.False(ReferenceEquals(value, clone));
        Assert.True(value.Item1 == clone.Item1);
        Assert.True(value.Item2 == clone.Item2);
    }

    [Fact]
    public void CloneList()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        var clone = Muq.Clone(list);
        Assert.False(ReferenceEquals(list, clone));
        Assert.True(EnumerableEqualityComparer<int>.Default.Equals(list, clone));

        for (var i = 0; i < list.Count; i++)
        {
            list[i] = i * 2;
        }

        Assert.False(EnumerableEqualityComparer<int>.Default.Equals(list, clone));
    }

    [Fact]
    public void CloneDictionary()
    {
        var dict = new Dictionary<string, object?>
        {
            { "Flags", BindingFlags.Static },
            { "Where?", "Joe's" },
            { "default", null },
            { "deeper", new List<int>(Enumerable.Range(1, 10)) },
            { "still", new Exception("TEST") },
        };
        var clone = Muq.Clone(dict);
        Assert.False(ReferenceEquals(dict, clone));
        Assert.True(dict.Count == clone.Count);
        foreach (var (dictKey, dictValue) in dict)
        {
            var got = clone.TryGetValue(dictKey, out var cloneValue);
            Assert.True(got);
            if (cloneValue == null)
            {
                Assert.True(dictValue == null);
            }
            else
            {
                Assert.False(ReferenceEquals(dictValue, cloneValue));
                Assert.True(dictValue?.GetType() == cloneValue.GetType());
                Assert.True(EqualityCache.Equals(dictValue, cloneValue));
            }
        }
        
    }
}